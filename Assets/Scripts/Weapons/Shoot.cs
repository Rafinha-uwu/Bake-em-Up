using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Dispara : MonoBehaviour
{
    public GameObject projetil;

    [SerializeField] private float Force;

    private bool cooldown;
    [SerializeField] private float CooldownTime = 0.5f;
    private float CoolTime = 0.5f;

    public void Start()
    {
        CoolTime = CooldownTime;
    }

    public void Update()
    {
        if (CoolTime > 0)
        {
            CoolTime -= Time.deltaTime;
            cooldown = true;
        }
        else if (CoolTime <= 0f)
        {
            cooldown = false;
        }
    }


    public void OnDispara()
    {
        if (!cooldown)
        {
            GameObject ProjTemp = Instantiate(projetil);


            ProjTemp.transform.SetParent(this.transform);
            ProjTemp.transform.localPosition = new Vector3(0f, 0f, 0.227f);
            ProjTemp.transform.rotation = this.transform.rotation;
            ProjTemp.transform.SetParent(null);

            ProjTemp.GetComponent<Rigidbody>().AddForce(ProjTemp.transform.forward * Force);

            CoolTime = CooldownTime;
        }
    }
}