using System.Collections.Generic;
using UnityEngine;
using Utils.Inspector;

namespace Audio
{
    //Save the location of all audio files, to load in runtime, save also the audio clip loaded when game launched
    [CreateAssetMenu(fileName = "AudioReferences", menuName = "Audio/AudioReferences", order = 1)]
    public class AudioReferences : ScriptableObject
    {
        public string[] AudioFilesPaths;

        [SerializeField]
        internal AudioClip[] clips = new AudioClip[0];
    }
}