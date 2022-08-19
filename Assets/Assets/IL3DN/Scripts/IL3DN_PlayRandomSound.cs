namespace IL3DN
{
    using UnityEngine;

    /// <summary>
    /// Plays a random shound on target audiosource
    /// Used to play sounds on objects
    /// </summary>
    public class IL3DN_PlayRandomSound : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip[] sounds;
        public float minDelay;
        public float maxDelay;
        float currentTime;
        float playTime;
        AudioClip currentSound;

        private void Start()
        {
            SetupSound();
        }

        /// <summary>
        /// Prepare the sound to be played next
        /// </summary>
        private void SetupSound()
        {
            currentSound = sounds[Random.Range(0, sounds.Length)];
            playTime = Random.Range(minDelay, maxDelay);
            currentTime = 0;
        }

        /// <summary>
        /// when time expires play the sound and setup the next one
        /// </summary>
        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime > playTime)
            {
                SetupSound();
                audioSource.PlayOneShot(currentSound);
            }
        }
    }
}
