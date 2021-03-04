using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuObject : MonoBehaviour
{
    public Text HostIPText;
    public Text FeedbackIndicatorText;

    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Exit_Click()
    {
        Application.Quit();
    }

    public void Play_Click()
    {
        try
        {
            ConnectionInstance.Instance = new ConnectionInstance(GetConnectionInfo());
            FeedbackIndicatorText.text = "Connected !";
        }
        catch (Exception ex)
        {
            FeedbackIndicatorText.text = "Failed : " + ex.Message;
        }
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void Host_Click()
    {
        //_server = new TcpListener(IPAddress.Any, 1212);
        //_server.Start();
        //var thread = new Thread(AcceptClients);
        //thread.Start(_server);
    }

    public void Join_Click()
    {
        try
        {
            //var endPoint = GetConnectionInfo();
            //ConnectionInstance.Instance = new PlayerModalConnector().Connect(endPoint);
            //FeedbackIndicatorText.text = "Connected !";
        }
        catch (Exception ex)
        {
            FeedbackIndicatorText.text = "Failed : " + ex.Message;
        }
    }

    private IPEndPoint GetConnectionInfo()
    {
        var connectionString = HostIPText.text;
        var connectionInfo = connectionString.Split(':');
        if (connectionInfo.Length != 2)
        {
            throw new Exception("Please enter valid ip:port or ill fuck your mom");
        }

        var ip = connectionInfo[0];
        if (!Regex.IsMatch(ip, @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}"))
        {
            throw new Exception("Bad ip");
        }

        if (!short.TryParse(connectionInfo[1], out var port))
        {
            throw new Exception("Bad port");
        }

        return new IPEndPoint(IPAddress.Parse(ip), port);
    }
}
