using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using System.Data.SqlServerCe;
using DatabaseConnectivity;
using System.IO;



public class Test : MonoBehaviour {
    public Text email;
    public Text mobileNo;
    string query;
    SqlCeConnection sqlCeConnection;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update ()
    {	
	}

    public void Connection()
    {
        print(Application.streamingAssetsPath);
        Class1 dataBase=new Class1();
        String connectioString=@"Data Source="+Application.streamingAssetsPath+"/Database.sdf;Password=password";
        sqlCeConnection=dataBase.Connectivity(connectioString);
        print(sqlCeConnection.State);
        
        SqlCeCommand cmd = sqlCeConnection.CreateCommand();
        SqlCeDataReader reader = cmd.ExecuteReader();
        cmd.CommandText = "Insert into Kingsmenpuzzle (Email,Mobile) Values ('"+email.text+"','"+mobileNo.text+"');";
        int i=cmd.ExecuteNonQuery();    
        print(i);                
    }
}



