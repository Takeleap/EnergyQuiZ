using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data.SqlServerCe;
using DatabaseConnectivity;
using System.Data;
using System.Linq;
using UnityEngine.SceneManagement;

public class ExperienceManager : MonoBehaviour {

    public bool isEnglish = true;

    private int englishCode = 1, arabicCode = 0;

    public GameObject[] arabgicObjects, englishObjects;

    public GameObject formObj, QuestionsPanel, LeaderBoard,gameover;

    public GameObject leaderboardOBJ, LeaderBoardPanel;

    public Button changeLanguageButton;

    public string connectioString = "",username;

    public Sprite toggleON, toggleOFF;

    public InputField name, mobile, email, namearab, mobilearab, emailarab;

    public int timetaken=0;

    public int score=0;

    public int userid;

    public bool QuestionsAnwered;

    public int[] questionSelected;

    public Text questionstext, questiontextArab;

    public int currentAnswerIndex;

    public Text finalNameDisplay,finalScoreDisplay,questionScoreDisplay,scoreDisplayNO,scoreDisplayNoFinal;

    public Button option1, option2, option3, option4,option5, option1arab, option2arab, option3arab, option4arab,option5arab;

    public Sprite[] rankColors;

    public bool idleScreenPlaying = false;

    public struct questions
    {
        public int id;
        public string question;
        public string[] options;
        public int answer;
        public Sprite[] optionicons;
    }

    public Dictionary<int, questions> QuestionsEnglish;
    public Dictionary<int, questions> QuestionsArabic;

    void Start ()
    {
        generateRandom();
        QuestionsAnwered = false;
        connectioString = @"Data Source=" + Application.streamingAssetsPath + "/Database.sdf;Password=takeleap@123!@#";
        QuestionsEnglish = new Dictionary<int, questions>();
        QuestionsArabic = new Dictionary<int, questions>();
        CheckLanguageSettings();
        StartCoroutine(LoadQuestions());
        StartCoroutine(LoadQuestionsArabic());
        
    }

    public IEnumerator timer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            timetaken++;
        }
    }

     void Update()
    {
        if(idleScreenPlaying)
        {
            if(Input.touchCount>0||Input.GetMouseButtonDown(0))
            {
                print("interacted");
                Reload();
                idleScreenPlaying = false;
            }
        }
    }


    public void CheckLanguageSettings()
    {
        print(PlayerPrefs.GetInt("IsEnglish") + " on main");
        if (PlayerPrefs.GetInt("IsEnglish") == 1)
        {
            isEnglish = true;
            PlayerPrefs.SetInt("IsEnglish", englishCode);
            PlayerPrefs.Save();
            changeLanguageButton.GetComponentInChildren<Image>().sprite = toggleOFF;
            changeLanguageButton.transform.GetChild(1).gameObject.SetActive(false);
            changeLanguageButton.transform.GetChild(0).gameObject.SetActive(true);
            foreach (GameObject englishitem in englishObjects)
            {
                englishitem.SetActive(true);
            }
            foreach (GameObject arabitem in arabgicObjects)
            {
                arabitem.SetActive(false);
            }
        }
        else if (PlayerPrefs.GetInt("IsEnglish") == 0)
        {
            isEnglish = false;
            PlayerPrefs.SetInt("IsEnglish", arabicCode);
            PlayerPrefs.Save();
            changeLanguageButton.GetComponentInChildren<Image>().sprite = toggleON;
            changeLanguageButton.transform.GetChild(0).gameObject.SetActive(false);
            changeLanguageButton.transform.GetChild(1).gameObject.SetActive(true);
            foreach (GameObject arabitem in arabgicObjects)
            {
                arabitem.SetActive(true);
            }
            foreach (GameObject englishitem in englishObjects)
            {
                englishitem.SetActive(false);
            }
        }
    }

    public void SwitchLanguage()
    {
        if (isEnglish == true)
        {
            PlayerPrefs.SetInt("IsEnglish", arabicCode);
            PlayerPrefs.Save();
            changeLanguageButton.GetComponentInChildren<Image>().sprite = toggleON;
            changeLanguageButton.transform.GetChild(0).gameObject.SetActive(false);
            changeLanguageButton.transform.GetChild(1).gameObject.SetActive(true);
            foreach (GameObject arabitem in arabgicObjects)
            {
                arabitem.SetActive(true);
            }
            foreach (GameObject englishitem in englishObjects)
            {
                englishitem.SetActive(false);
            }
            isEnglish = false;
        }
        else
        {
            PlayerPrefs.SetInt("IsEnglish", englishCode);
            PlayerPrefs.Save();
            changeLanguageButton.GetComponentInChildren<Image>().sprite = toggleOFF;
            changeLanguageButton.transform.GetChild(1).gameObject.SetActive(false);
            changeLanguageButton.transform.GetChild(0).gameObject.SetActive(true);
            foreach (GameObject englishitem in englishObjects)
            {
                englishitem.SetActive(true);
            }
            foreach (GameObject arabitem in arabgicObjects)
            {
                arabitem.SetActive(false);
            }
            isEnglish = true;
        }
        print(" PlayerPrefs " + PlayerPrefs.GetInt("IsEnglish"));
    }


    public void Submit()
    {
        string insertquerry = "";
        if(isEnglish)
        {
            if(name.text.Length>2)
            {
                insertquerry = "insert into userinfo(username,emailid,mobileno,timetaken,score) " +
                  "values('" + name.text + "','" + email.text + "','" + mobile.text + "','0','0')";
                username = name.text;
            }
            else
            {
                return;
            }
            
        }
        else
        {
            if(namearab.text.Length>2)
            {
                insertquerry = "insert into userinfo(username,emailid,mobileno,timetaken,score) " +
               "values('" + namearab.text + "','" + emailarab.text + "','" + mobilearab.text + "','0','0')";
                username = namearab.text;
            }
            else
            {
                return;
            }
     
        }
        print(insertquerry);
        SqlCeConnection dbconn = new SqlCeConnection(connectioString);
        dbconn.Open();
        SqlCeCommand insertCMD = dbconn.CreateCommand();
        insertCMD.CommandText = insertquerry;
        insertCMD.ExecuteNonQuery();
        dbconn.Close();
        string getid= "select TOP(1)userid from userinfo order by userid desc";
        dbconn.Open();
        SqlCeCommand getidCMD = dbconn.CreateCommand();
        insertCMD.CommandText = getid;
        SqlCeDataReader reader = insertCMD.ExecuteReader();
        while(reader.Read())
        {
            userid = reader.GetInt32(0);
        }
        StartCoroutine(timer());
        StartCoroutine(AskQuestions());
        dbconn.Close();
    }

    public IEnumerator LoadQuestions()
    {
        DataTable data = new DataTable();
        string fetchEnglishQuestions = "select questionid,questiontext,option1,option2,option3,option4,anwer,option5 from questions";
        SqlCeConnection dbconn = new SqlCeConnection(connectioString);
        dbconn.Open();
        SqlCeCommand fetchQuestion = dbconn.CreateCommand();
        fetchQuestion.CommandText = fetchEnglishQuestions;
        SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(fetchQuestion);
        dataAdapter.Fill(data);
        foreach(DataRow row in data.Rows)
        {
            print(row[0].ToString() + "" + row[1].ToString() + row[2].ToString() + row[3].ToString() + row[4].ToString() + row[5].ToString() + row[6].ToString() + row[7].ToString());
            questions temp = new questions();
            temp.id = Int32.Parse(row[0].ToString());
            temp.question = row[1].ToString();
            temp.options = new string[5];
            temp.options[0] = row[2].ToString();
            temp.options[1] = row[3].ToString();
            temp.options[2] = row[4].ToString();
            temp.options[3] = row[5].ToString();
            temp.options[4] = row[7].ToString();
            temp.answer = Int32.Parse(row[6].ToString());
            QuestionsEnglish.Add(Int32.Parse(row[0].ToString()), temp);
        }
        for(int i=1;i<=10;i++)
        {
            questions temp = new questions();
            if(QuestionsEnglish.TryGetValue(i,out temp))
            {
                print(temp.question+"from struct");
            }
        }
        yield return null;
    }

    public IEnumerator LoadQuestionsArabic()
    {
        DataTable data = new DataTable();
        string fetchEnglishQuestions = "select questionid,questiontextarab,option1arab,option2arab,option3arab,option4arab,anwer,option5arab from questions";
        SqlCeConnection dbconn = new SqlCeConnection(connectioString);
        dbconn.Open();
        SqlCeCommand fetchQuestion = dbconn.CreateCommand();
        fetchQuestion.CommandText = fetchEnglishQuestions;
        SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(fetchQuestion);
        dataAdapter.Fill(data);
        foreach (DataRow row in data.Rows)
        {
            print(row[0].ToString() + "" + row[1].ToString() + row[2].ToString() + row[3].ToString() + row[4].ToString() + row[5].ToString() + row[6].ToString() + row[7].ToString());
            questions temp = new questions();
            temp.id = Int32.Parse(row[0].ToString());
            temp.question = row[1].ToString();
            temp.options = new string[5];
            temp.options[0] = row[2].ToString();
            temp.options[1] = row[3].ToString();
            temp.options[2] = row[4].ToString();
            temp.options[3] = row[5].ToString();
            temp.options[4] = row[7].ToString();
            temp.answer = Int32.Parse(row[6].ToString());
            QuestionsArabic.Add(Int32.Parse(row[0].ToString()), temp);
        }
        for (int i = 1; i <= 10; i++)
        {
            questions temp = new questions();
            if (QuestionsArabic.TryGetValue(i, out temp))
            {
                print(temp.question + "from struct");
            }
        }
        yield return null;
    }

    public void generateRandom()
    {
        questionSelected = new int[10];
        var rng = new System.Random();
        var values = Enumerable.Range(0, 29).OrderBy(x => rng.Next()).ToArray();
        for (int i = 0; i < 10; i++)
        {
            questionSelected[i] = values[i];
            print("Random  " + questionSelected[i]);
        }
    }

    public IEnumerator AskQuestions()
    {
        formObj.SetActive(false);
        QuestionsPanel.SetActive(true);
        foreach(int i in questionSelected)
        {
            questions temp;
            QuestionsAnwered = false;
            if(isEnglish)
            {
                if(QuestionsEnglish.TryGetValue(i,out temp))
                {
                    questionstext.text = temp.question;
                    option1.GetComponentInChildren<Text>().text = temp.options[0];
                    option2.GetComponentInChildren<Text>().text = temp.options[1];
                    option3.GetComponentInChildren<Text>().text = temp.options[2];
                    option4.GetComponentInChildren<Text>().text = temp.options[3];
                    option5.GetComponentInChildren<Text>().text = temp.options[4];
                    currentAnswerIndex = temp.answer;
                }
            }
            else
            {
                if (QuestionsArabic.TryGetValue(i, out temp))
                {
                    questiontextArab.text = temp.question;
                    option1arab.GetComponentInChildren<Text>().text = temp.options[0];
                    option2arab.GetComponentInChildren<Text>().text = temp.options[1];
                    option3arab.GetComponentInChildren<Text>().text = temp.options[2];
                    option4arab.GetComponentInChildren<Text>().text = temp.options[3];
                    option5arab.GetComponentInChildren<Text>().text = temp.options[4];
                    currentAnswerIndex = temp.answer;
                }
            }
            yield return new WaitUntil(() => QuestionsAnwered == true);
            scoreDisplayNO.text = score.ToString();
            questionScoreDisplay.text = score + "/10";
        }
        QuestionsPanel.SetActive(false);
        
        EvaluateAnswer();
    }


    public void EvaluateAnswer()
    {
        gameover.SetActive(true);
        finalNameDisplay.text = "Thank you, "+username+"!";
        scoreDisplayNoFinal.text = score.ToString();
        finalScoreDisplay.text = score + "/10";

        string query = "Update userinfo set score = '" + score + "' ,timetaken = '" + timetaken + "' where userid = '" + userid + "' ";
        SqlCeConnection dbconn = new SqlCeConnection(connectioString);
        dbconn.Open();
        SqlCeCommand insertCMD = dbconn.CreateCommand();
        insertCMD.CommandText = query;
        insertCMD.ExecuteNonQuery();
    
    }

    public void LeaderBoardDisplay()
    {
        gameover.SetActive(false);
        LeaderBoard.SetActive(true);
        getLeaderBoard();
    }

    public void getLeaderBoard()
    {
        int rank = 1;
        idleScreenPlaying = true;
        string query = "select TOP(5)* from userinfo order by score DESC";
        DataTable data = new DataTable();
        SqlCeConnection dbconn = new SqlCeConnection(connectioString);
        dbconn.Open();
        SqlCeCommand insertCMD = dbconn.CreateCommand();
        SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(insertCMD);
        insertCMD.CommandText = query;
        sqlCeDataAdapter.Fill(data);
        foreach(Transform child in LeaderBoardPanel.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        foreach(DataRow row in data.Rows)
        {
            GameObject temp = Instantiate(leaderboardOBJ, LeaderBoardPanel.transform);
            temp.transform.GetChild(2).GetComponent<Image>().sprite = rankColors[rank - 1];
            temp.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = rank.ToString();
            temp.transform.GetChild(3).GetComponent<Text>().text = row[1].ToString();
            temp.transform.GetChild(4).GetComponent<Text>().text = row[5].ToString()+" pts";
            rank++;
        }
    }

    public void SelectedOption(int index)
    {
        print(index + " " + currentAnswerIndex);
        if(index==currentAnswerIndex)
        {
            score++;
            QuestionsAnwered = true;
        }
        else
        {
            QuestionsAnwered = true;
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
