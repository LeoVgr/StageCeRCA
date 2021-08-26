using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Load all audio clip, in the folder of application and set all in the scriptable object AudioReferences, in runtime.
    /// </summary>
    public class AudioStarter : MonoBehaviour
    {
        public AudioReferences References;
        public GameObject PrefabImporter;

        private static bool _launched = false;

        // Start is called before the first frame update
        void Start()
        {
            if (_launched)
            {
                Debug.LogWarning("Sounds already loaded ???");
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            References.clips = new AudioClip[References.AudioFilesPaths.Length];

            for (int i = 0; i < References.AudioFilesPaths.Length; i++)
            {
                Instantiate(PrefabImporter);
            }

            _launched = true;
            //Destroy(this.gameObject);
        }

        // private void Update()
        // {
        //     if (_importer.isError)
        //     {
        //         Debug.LogWarning("Audio Files " + References.AudioFilesPaths[_index] + " not found !");
        //
        //         //Try with another sound files, if not have other kill the process
        //         _index++;
        //         if (_index >= References.AudioFilesPaths.Length)
        //         {
        //             _importer.Abort();
        //             Destroy(gameObject);
        //         }
        //         else
        //         {
        //             _importer.Abort();
        //             Import();
        //         }
        //     }
        //     else if (_importer.isDone)
        //     {
        //         var clip = _importer.audioClip;
        //         Debug.Log("Audio " + clip.name + " loaded, lenght=" + clip.length);
        //         //Save the clip
        //         AudioClip audioClip = clip;
        //         if (References.clips.Count < _index+1)
        //             References.clips.Add(new List<AudioClip>());
        //         References.clips[_index++].Add(audioClip);
        //
        //         //Load next clip
        //         if (_index < References.AudioFilesPaths.Length)
        //         {
        //             Import();
        //         }
        //         else
        //         {
        //             Debug.Log("All Audio files loaded.");
        //             Destroy(gameObject);
        //         }
        //     }
        // }
    }
}