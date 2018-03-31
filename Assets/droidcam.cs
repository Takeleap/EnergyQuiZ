using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class droidcam : MonoBehaviour {

    WebCamDevice device;

    public RawImage quad;

    public WebCamTexture webCam;
	// Use this for initialization
	void Start () {
        
        WebCamDevice[] device = WebCamTexture.devices;
        for(int i=0;i<device.Length;i++)
        {
            print(device[i].name);
        }
        webCam = new WebCamTexture(device[0].name,1920,1080,30);
        quad.texture = webCam;
        webCam.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
