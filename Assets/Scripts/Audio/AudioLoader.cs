using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Get the the audio clip loaded and add in runtime the audio clip in the audio source
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioLoader : MonoBehaviour
    {
        public AudioReferences References;
        [Tooltip("Look in audio references, the index reference the sound to come play")]
        public int SoundIndex;
        public AudioSource Source;
        public FloatVariable MusicVolume;
        public FloatVariable SfxVolume;
        public bool IsSfxSound;

        private void Start()
        {
            if (Source == null)
                Source = GetComponent<AudioSource>();

            if (References.clips.Count <= SoundIndex)
            {
                Debug.LogError("sound file at path : " + Application.dataPath + "/" +
                               References.AudioFilesPaths[SoundIndex] + " not found ! GameObject : " + gameObject.name +
                               " in scene : " + gameObject.scene.name);
                return;
            }

            //Set the clip loaded in audio source
            AudioClip referencesClip = References.clips[SoundIndex];

            Source.clip = referencesClip;
            if (Source.playOnAwake)
                Source.Play();

            //Adjust the sound of the clip depending options
            if (!IsSfxSound)
            {
                Source.volume = MusicVolume.Value;
            }
            else
            {
                Source.volume = SfxVolume.Value;
            }
        }
    }
}