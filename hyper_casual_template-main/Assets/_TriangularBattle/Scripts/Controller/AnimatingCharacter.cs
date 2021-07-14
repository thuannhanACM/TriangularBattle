using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingCharacter : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    public static int[] animationsIndex = new int[] { 1, 2, 3, 4, 5};
    private float changeAnimTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        ChangeAnim();
    }

    // Update is called once per frame
    void Update()
    {
        changeAnimTime-=Time.deltaTime;
        if(changeAnimTime<=0.0f)
        {
            changeAnimTime=10.0f;
            ChangeAnim();
        }
    }

    void ChangeAnim()
    {
        int r = Random.Range(0, animationsIndex.Length);
        animator.SetInteger("Anim", r+1);
        animator.SetTrigger("Switch");
    }

    public void ChangeMaterial(Material mat)
    {
        skinnedMeshRenderer.material=mat;
    }
}
