using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    private Animator anim;

    public delegate void Function();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Transition(Function FuncToCall = null)
    {
        anim.SetTrigger("Into");

        if (FuncToCall != null) StartCoroutine(CallFunction(FuncToCall));
    }

    public IEnumerator CallFunction(Function FuncToCall)
    {
        yield return new WaitForSeconds(1f);

        FuncToCall();

        yield return new WaitForSeconds(3f);

        anim.SetTrigger("Outof");
    }
}
