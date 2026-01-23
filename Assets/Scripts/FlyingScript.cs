using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class FlyingScript : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float rotationSpeed = 2;
    [SerializeField] float yaw = 0;
    [SerializeField] float pitch = 0;
    [SerializeField] Animator anim;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse movement
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch += Input.GetAxis("Mouse Y") * rotationSpeed;

        //limit vertical rotation to avoid féipping
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //apply rotation to transform
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        //move forward automatically
        transform.position += transform.forward * speed * Time.deltaTime;

        //quitting
        if(Input.GetKeyUp(KeyCode.Escape))
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
        anim.SetBool("Fading", true);
        yield return new WaitForSeconds(1);
        yaw = 0;
        pitch = 0;
        transform.position = new Vector3(215, 70, 260);
        anim.SetBool("Fading", false);
    }
}
