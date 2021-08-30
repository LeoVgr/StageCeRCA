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
        
        public bool IsSfxSound;

        private void Start()
        {
            if (Source == null)
                Source = GetComponent<AudioSource>();
            
            //Adjust the sound of the clip depending options
            if (!IsSfxSound)
            {
                if (DataManager.instance.MusicVolume)
                    Source.volume = DataManager.instance.MusicVolume.Value;
            }
            else
            {
                if (DataManager.instance.SfxVolume)
                    Source.volume = DataManager.instance.SfxVolume.Value;
            }
            
            if (References.clips.Length <= SoundIndex || References.clips[SoundIndex] == null)
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
        }

        public void SetVolumeMusic(float value)
        {
            DataManager.instance.MusicVolume.Value = value;
        }
        public void SetSfxVolumeMusic(float value)
        {
            DataManager.instance.SfxVolume.Value = value;
        }
    }
}