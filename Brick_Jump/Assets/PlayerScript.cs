using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public GameManagerScript gm;

    public AudioClip bounce, death, win;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float speed = 7;
    private bool onGround = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("SoundFX"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("SoundFX", 100);
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameRunning)
        {
            rb.gravityScale = 1f;
            rb.velocity = new Vector2(speed, rb.velocity.y);
            speed += 0.05f * Time.deltaTime;

            if (Input.GetMouseButton(0) && onGround && !EventSystem.current.IsPointerOverGameObject())
            {
                //audioSource.PlayOneShot(bounce);
                rb.velocity = new Vector2(rb.velocity.x, 5f);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag == "ground")
        {
            onGround = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "ground")
        {
            onGround = true;
        }

        if(collision.collider.tag == "deathCollider")
        {
            //audioSource.PlayOneShot(death);
            gm.GameOver = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "star")
        {
            gm.CollectStar(collision.gameObject);        
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "flag")
        {
            //audioSource.PlayOneShot(win);
            gm.LevelComplete = true;
            int currentLevelNum = int.Parse(SceneManager.GetActiveScene().name.Replace("Level", ""));
            int levelUnlockedNum = PlayerPrefs.GetInt("LevelUnlocked", 1);

            if(currentLevelNum + 1 > levelUnlockedNum)
            {
                PlayerPrefs.SetInt("LevelUnlocked", int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")) + 1);
                PlayerPrefs.Save();
            }         
        }
    }
}
