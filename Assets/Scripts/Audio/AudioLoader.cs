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

        private void Start()
        {
            if (Source == null)
                Source = GetComponent<AudioSource>();

            if (References.clips.Count <= SoundIndex /*|| References.clips[SoundIndex].Count == 0*/)
            {
                Debug.LogError("sound file at path : " + Application.dataPath + "/" +
                               References.AudioFilesPaths[SoundIndex] + " not found ! GameObject : " + gameObject.name +
                               " in scene : " + gameObject.scene.name);
                return;
            }

            //Set the clip loaded in audio source
            //var index = Random.Range(0, References.clips[SoundIndex].Count);
            AudioClip referencesClip = References.clips[SoundIndex];

            Source.clip = referencesClip;
            if (Source.playOnAwake)
                Source.Play();
        }
    }
}