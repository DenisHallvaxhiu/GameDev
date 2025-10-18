using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorLevelExit : MonoBehaviour {
    public enum EnterMode { AutoOnTrigger, PressToEnter, OnCollision }

    //Different type of door entry.. we can discuss on what we prefer
    public EnterMode mode = EnterMode.OnCollision;

    //To play a SFX or an animation
    public float loadDelay = 0.25f;


    //public bool playOpenAnimation = true;
    //public string animatorTriggerName = "Open";
    //public AudioSource sfx;

    [Header("Player detection")]
    public string playerTag = "Player";

    // used for PressToEnter
    bool canEnter;
    bool loading;


    //For AutoOnTrigger and Press to enter
    //void OnTriggerEnter2D(Collider2D other) {
    //    if(!other.CompareTag(playerTag)) return;
    //    if(mode == EnterMode.AutoOnTrigger) TryEnter();
    //    else if(mode == EnterMode.PressToEnter) canEnter = true;
    //}
    //void OnTriggerExit2D(Collider2D other) {
    //    if(mode == EnterMode.PressToEnter && other.CompareTag(playerTag))
    //        canEnter = false;
    //}

    void OnCollisionEnter2D(Collision2D col) {
        if(mode == EnterMode.OnCollision && col.collider.CompareTag(playerTag))
            TryEnter();
    }



    void TryEnter() {
        //if(loading) return;
        //loading = true;


        //Animation/Music
        //if(playOpenAnimation) {
        //    var anim = GetComponent<Animator>();
        //    if(anim) anim.SetTrigger(animatorTriggerName);
        //}

        //if(sfx) sfx.Play();
        Invoke(nameof(LoadNextScene),loadDelay);
    }

    void LoadNextScene() {
        int current = SceneManager.GetActiveScene().buildIndex;
        int count = SceneManager.sceneCountInBuildSettings;

        // Loads the next scene; if current is last, wraps to 0 (optional).
        int next = (current + 1 < count) ? current + 1 : 0;
        SceneManager.LoadScene(next);
    }
}
