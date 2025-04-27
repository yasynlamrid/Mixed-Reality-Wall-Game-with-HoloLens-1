using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnProjectile : MonoBehaviour
{
    public GameObject firepoint;
    public List<GameObject> vfx = new List<GameObject>();
    public RotateToMouse rotate;
    public Text effectName;

    private GameObject effectToSpawn;
    private float timeToFire = 0;
    private int number = 0;
    void Start()
    {
        if (vfx.Count > 0)
        {
            effectToSpawn = vfx[0];
        }
        else
        {
            Debug.Log("Assign 1 or more VFXs");
        }

        if (effectName != null && vfx.Count > 0)
        {
            effectName.text = effectToSpawn.name;
        }
    }


    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
            SpawnVFX();
        }


        if (Input.GetKeyDown(KeyCode.D))
            Next();

        if (Input.GetKeyDown(KeyCode.Q))
            Previous();
    }

    void SpawnVFX()
    {
        GameObject vfx;

        if (firepoint != null)
        {
            vfx = Instantiate(effectToSpawn, firepoint.transform.position, Quaternion.identity);
            if (rotate != null)
            {
                vfx.transform.localRotation = rotate.GetRotation();
            }
        }

        else
        {
            Debug.Log("No Fire Point");
        }
    }

    public void Next()
    {
        number++;

        if (number > vfx.Count)
            number = 0;

        for (int i = 0; i < vfx.Count; i++)
        {
            if (number == i) effectToSpawn = vfx[i];
            if (effectName != null) effectName.text = effectToSpawn.name;
        }
    }

    public void Previous()
    {
        number--;

        if (number < 0)
            number = vfx.Count;

        for (int i = 0; i < vfx.Count; i++)
        {
            if (number == i) effectToSpawn = vfx[i];
            if (effectName != null) effectName.text = effectToSpawn.name;
        }
    }
}
