using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FlyingScript : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float rotationSpeed = 2;
    [SerializeField] float yaw = 0;
    [SerializeField] float pitch = 0;
    [SerializeField] Animator anim;

    [SerializeField] private List<GameObject> CheckpointList;
    [SerializeField] private int CheckpointCounter = -1;
    [SerializeField] Transform checkPointHolder;

    //új változók:
    [SerializeField] public TMP_Text timerText;
    float timeLeft = 10;
    float finishedTime;
    bool round = false;

    Vector3 spawnPos = new Vector3(215, 70, 260);

    private void Awake()
    {
        for(int i = 0; i < checkPointHolder.childCount; i++)
        {
            CheckpointList.Add(checkPointHolder.GetChild(i).gameObject);
            CheckpointList[i].SetActive(false);
        }
    }

    void Start()
    {
        round = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckpointHandler();
        CountDown();
    }
    void CountDown()
    {
        if(timeLeft > 0 && round)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = (Mathf.Round(timeLeft)).ToString();
        }
        else
        {
            StartCoroutine(Restart());
        }
    }
    IEnumerator Restart()
    {
        anim.SetBool("Fading", true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // restarts the scene
    }

    void CheckpointHandler()
    {
        if (CheckpointCounter + 1 < CheckpointList.Count)
        {
            CheckpointList[CheckpointCounter + 1].SetActive(true);
        }
        if(CheckpointCounter > -1)
        {
            spawnPos = CheckpointList[CheckpointCounter].transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Respawn":
                CheckpointCounter++;
                other.transform.parent.gameObject.SetActive(false);
                timeLeft += 10;
                break;
        }
        if(CheckpointCounter == CheckpointList.Count - 1)
        {
            WIN();
        }
    }

    void Movement()
    {
        //Get mouse movement
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch += Input.GetAxis("Mouse Y") * rotationSpeed;

        //limit vertical rotation to avoid féipping
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //apply rotation to transform
        transform.rotation = Quaternion.Euler(-pitch, yaw, 0);

        //move forward automatically
        transform.position += transform.forward * speed * Time.deltaTime;

        //quitting
        if (Input.GetKeyUp(KeyCode.Return))
        {
            StartCoroutine(Quit());
        }
    }

    IEnumerator Quit()
    {
        anim.SetBool("Fading", true);
        yield return new WaitForSeconds(1.5f);
        Cursor.lockState = CursorLockMode.None;
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    private void OnCollisionEnter(Collision col)
    {
        StartCoroutine(Collided());
    }

    IEnumerator Collided()
    {
        speed = 0;
        anim.SetBool("Fading", true);
        yield return new WaitForSeconds(1);
        yaw = 0;
        pitch = 0;
        transform.position = spawnPos; //alpaeset: Vector3(215, 70, 260);
        speed = 10;
        anim.SetBool("Fading", false);
    }

    void WIN()
    {
        round = false;
        finishedTime = timeLeft;
        timerText.text = "Yay u won! time: " + finishedTime;
    }
}
