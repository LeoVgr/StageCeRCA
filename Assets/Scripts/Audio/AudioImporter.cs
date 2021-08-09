using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioImporter : MonoBehaviour
    {
        public AudioReferences References;
        public global::AudioImporter Importer;
        private static int index = 0;
        private int _myIndex;
        private string path =>  Application.dataPath + "/" + References.AudioFilesPaths[_myIndex];

        // Start is called before the first frame update
        void Start()
        {
            _myIndex = index++;
            DontDestroyOnLoad(gameObject);

            Debug.Log("Start loading audio file "+path);
            Importer.Loaded += OnLoaded;
            Importer.Import(path);
        }

        
        private void OnLoaded(AudioClip clip)
        {
            Debug.Log("Loading audio file "+path);
            References.clips.Insert(_myIndex,clip);
        }

        private void Update()
        {
            if (Importer.isDone)
            {
                Debug.Log("Audio file "+path+" completely loaded.");
                Importer.Abort();
                Destroy(this.gameObject);
            }
            else if (Importer.isError)
            {
                Debug.LogError("Audio file "+path+" error during loading.");
                Importer.Abort();
                Destroy(this.gameObject);
            }
        }
    }
}