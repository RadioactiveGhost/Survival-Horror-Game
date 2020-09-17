using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeMoviment : MonoBehaviour
{
    private Vector3 target;
    //private bool resetEyeRot = false;
    private Quaternion initialRotation;
    private float rotationResetSpeed = 20f; 
    public enum Eye { right, left};
    public Eye eye;

    EyeHandler eyehandler;

    private float minClampR;
    private float maxClampR;

    mobAi moBai;

    

    // Start is called before the first frame update
    void Awake()
    {
        moBai = GetComponentInParent<mobAi>();
        initialRotation = transform.localRotation;

        switch (eye)
        {
            case Eye.right:
                //minClampR = 0f;
                //maxClampR = 80;
                eyehandler = GameObject.Find("RightEyeManager").GetComponent<EyeHandler>();

                break;
            case Eye.left:
                eyehandler = GameObject.Find("LeftEyeManager").GetComponent<EyeHandler>();

                //minClampR = 40;
                //maxClampR = 170;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update() //75 a 125
    {

        if(moBai.isStaring == true && eyehandler.playercheck)
        {
            float angleZ = transform.localEulerAngles.z;
            float angleX = transform.localEulerAngles.x;
            float angleY = transform.localEulerAngles.y;
            //resetEyeRot = true;
            target = moBai.target;
            //transform.LookAt(target);

            //Vector3 direction = (target - transform.position).normalized;
            //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);


            Vector3 direction = (target - transform.position).normalized;


            //float clampedAngleZ = Mathf.Clamp(angleZ, 75, 125);
            //float clampedAngleX = Mathf.Clamp(angleX, 40, 80);
            //float clampedAngleY = Mathf.Clamp(angleY, minClampR, maxClampR);


            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, clampedAngleZ);
            // transform.localEulerAngles = new Vector3(clampedAngleX, clampedAngleY, clampedAngleZ);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);

        }
        else
        {
           
            Quaternion.Slerp(transform.localRotation, initialRotation, Time.time * rotationResetSpeed);
        }

  

    }

  
}
