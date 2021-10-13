using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{

    private Animator anim;
    private GameObject knife;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        knife = GameObject.FindGameObjectWithTag("Knife");
        knife.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Standing Idle") && Input.GetKeyDown(KeyCode.Space))
        {
            knife.SetActive(true);
            anim.SetTrigger("Stab");
        }
    }
}
