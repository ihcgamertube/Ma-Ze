using System;
using System.Collections.Generic;
using System.Net;
using GameNetwork.Client;
using GameNetwork.Core.Messages;
using GameNetwork.Core.Messages.Custom;
using GameNetwork.Core.Messages.Custom.Stubs;
using UnityEngine;

public class ConnectionInstance
{
    public static ConnectionInstance Instance;

    public ConnectionInstance(IPEndPoint endPoint)
    {
        Client = GameNetworkClient.Connect(endPoint);
    }

    public GameNetworkClient Client { get; }
}


public class GameSessionManager : MonoBehaviour
{
    private Dispatcher _dispatcher;

    public GameObject AnotherPlayer;
    //public GameObject Player;

    public Transform _myTransform;
    public Transform _otherTransform;

    void Awake()
    {
        _dispatcher = Dispatcher.FromCurrentThread();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        var position =
            new System.Numerics.Vector3(_myTransform.position.x, _myTransform.position.y, _myTransform.position.z);
        var rotation = new System.Numerics.Quaternion(_myTransform.rotation.x, _myTransform.rotation.y,
            _myTransform.rotation.z, _myTransform.rotation.w);

        ConnectionInstance.Instance.Client.SendCoordinates(position, rotation);
    }

    void Start()
    {
        //_myTransform = Player.transform;
        _otherTransform = Instantiate(AnotherPlayer).transform;
        ConnectionInstance.Instance.Client.SetMessageReceivedCallback(MessageReceived);
    }

    private void MessageReceived(Message message)
    {
        switch (message.MessageType)
        {
            case MessageType.ServerCoordinates:
                var coordinates = ByteConverters.FromBytes<ServerCoordinatesMessage>(message.Data);
                _dispatcher.Invoke(() =>
                {
                    for (var i = 0; i < coordinates.Count; i++)
                    {
                        if (coordinates.Coordinates[i].Player == ConnectionInstance.Instance.Client.Id)
                        {
                            //Debug.Log("Me : " + coordinates.Rotations[i]);
                            continue;
                        }
                        else
                        {
                            _otherTransform.position = new Vector3(
                                 coordinates.Coordinates[i].Position.X,
                                 coordinates.Coordinates[i].Position.Y,
                                 coordinates.Coordinates[i].Position.Z);
                            _otherTransform.rotation = new Quaternion(
                                coordinates.Coordinates[i].Rotation.X,
                                coordinates.Coordinates[i].Rotation.Y,
                                coordinates.Coordinates[i].Rotation.Z,
                                coordinates.Coordinates[i].Rotation.W);
                            //Debug.Log("Other : " + coordinates.Rotations[i]);
                        }
                    }
                });
                break;
        }

    }
}