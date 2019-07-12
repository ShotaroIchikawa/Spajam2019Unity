using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseRDB : MonoBehaviour
{

    // Firebase
    private DatabaseReference _FirebaseDB;
    private Firebase.Auth.FirebaseUser _FirebaseUser;


    void Start()
    {
        // Firebase RealtimeDatabase接続初期設定
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://spajam2019final.firebaseio.com/");
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        GetDatabaseData();

        FirebaseDatabase.DefaultInstance
        .GetReference("testNode")
        .ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        DataSnapshot snapShot = args.Snapshot;
        string json = snapShot.GetRawJsonValue();
        Debug.Log("Read: " + json);
    }

    private void GetDatabaseData()
    {
        Debug.Log("aaa");
        FirebaseDatabase.DefaultInstance.GetReference("testNode")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("not found users");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapShot = task.Result;
                    string json = snapShot.GetRawJsonValue();
                    Debug.Log("Read: " + json);
                    //Debug.Log(snapShot.Value);
                }
            });
    }

  
}