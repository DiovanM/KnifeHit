using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class TargetBehaviour : MonoBehaviour {
       
    [SerializeField]
    private int knifesToComplete;
    [SerializeField]
    private float velocity;
    private float initialVelocity;    
    private bool SlowdownOrAccelerate;
    [SerializeField]
    private bool WillVaryVelocity;
    [SerializeField]
    private float timeToVaryVelocity;
    private float lerpVariable;
    [SerializeField]
    private bool willHaveKnifes;
    [SerializeField]
    [Range(0,3)]
    private int knifeAmount;
    enum Patterns {Random, Degrees30, Degrees90, Degrees180For2Knifes, TriangleFor3Knifes};
    [SerializeField]
    Patterns pattern;
    [SerializeField]
    [Range(0,4)]
    private int appleAmount;
    [SerializeField]
    [Range(0, 1f)]
    private float appleSpawnChance;
    [SerializeField]
    private GameObject knifePrefab;
    [SerializeField]
    private GameObject applePrefab;
    private float circleRadius = 1.45f;

    private GameObject[] fixedKnifes = new GameObject[3];
    private GameObject[] apples = new GameObject[4];

    private int[] storeKnifesAngles = new int[3];

    private void OnEnable()
    {
        GameManager.levelScore += SetScore; //Passa o score necessário para passar de fase para o GameManager quando chamado
        if (willHaveKnifes && knifeAmount != 0)
        {
            SpawnKnifes((int)pattern);
        }
        if (Random.value <= appleSpawnChance)
        {
            SpawnApples();
        }
    }

    private void OnDisable()
    {
        DestroyKnifesandApples();
    }

    private void OnDestroy()
    {
        GameManager.levelScore -= SetScore;
    }       

    void Start()
    {
        
        SlowdownOrAccelerate = true;
        initialVelocity = velocity;
        lerpVariable = 0;
    }

    void Update()
    {
        Rotate();
        if (WillVaryVelocity)
        {
            VaryVelocity();
        }
    }

    private void Rotate()
    {
        GetComponent<Rigidbody2D>().angularVelocity = velocity * 10;
    }

    void VaryVelocity()
    {
        if (SlowdownOrAccelerate)
        {
            if (velocity > 0)
            {
                velocity = Mathf.Lerp(initialVelocity, 0, lerpVariable);
                lerpVariable += Time.deltaTime / timeToVaryVelocity;
            }
            else
            {
                lerpVariable = 0;
                SlowdownOrAccelerate = !SlowdownOrAccelerate;
            }
        }
        else
        {
            if (velocity < initialVelocity)
            {
                velocity = Mathf.Lerp(0, initialVelocity, lerpVariable);
                lerpVariable += Time.deltaTime / timeToVaryVelocity;
            }
            else
            {
                lerpVariable = 0;
                SlowdownOrAccelerate = !SlowdownOrAccelerate;
            }
        }
    }

    private void SpawnKnifes(int pattern)
    {        
        int angleToInstantiate = 0;
    
        int[] prevAngle = new int[4];
        prevAngle[0] = 0;

        for (int i = 0; i < knifeAmount; i++)
        {
            switch (pattern) //Define o angulo para instancionar a faca de acordo com o padrão estabelecido
            {
                case 0:
                    {
                        while (Mathf.Abs(angleToInstantiate - prevAngle[i]) < 10)
                        {
                            angleToInstantiate = Random.Range(0, 361);
                        }
                        prevAngle[i + 1] = angleToInstantiate;
                        break;
                    }
                case 1:
                    {
                        angleToInstantiate = i * 30;
                        break;
                    }
                case 2:
                    {
                        angleToInstantiate = i * 90;
                        break;
                    }
                case 3:
                    {
                        angleToInstantiate = i * 180;
                        break;
                    }
                case 4:
                    {
                        angleToInstantiate = i * 120;
                        break;
                    }
            }
            storeKnifesAngles[i] = angleToInstantiate;
            Vector2 knifePos;
            knifePos.x = circleRadius * Mathf.Sin(angleToInstantiate*Mathf.Deg2Rad);
            knifePos.y = circleRadius * Mathf.Cos(angleToInstantiate*Mathf.Deg2Rad);

            fixedKnifes[i] = Instantiate(knifePrefab, knifePos, Quaternion.identity) as GameObject;
            LookAt2D(fixedKnifes[i], transform);
            fixedKnifes[i].transform.SetParent(transform);
        }
            
    }

    private void SpawnApples()
    {
        int angleToInstantiate = 0;
        int[] prevAngle = new int[appleAmount+1];
        prevAngle[0] = 0;

        for (int i = 0; i < appleAmount; i++)
        {
            while ( Mathf.Abs(angleToInstantiate - prevAngle[i]) < 15 || Mathf.Abs(angleToInstantiate - storeKnifesAngles[i]) < 15)
            {
                angleToInstantiate = Random.Range(0, 361);
            }
            prevAngle[i + 1] = angleToInstantiate;
            Vector2 applePos;
            applePos.x = circleRadius * Mathf.Sin(angleToInstantiate * Mathf.Deg2Rad);
            applePos.y = circleRadius * Mathf.Cos(angleToInstantiate * Mathf.Deg2Rad);            

            apples[i] = Instantiate(applePrefab, applePos, Quaternion.identity) as GameObject;
            LookAt2D(apples[i], transform);
            apples[i].transform.SetParent(transform);
        }
    }

    private void DestroyKnifesandApples()
    {
        for (int i = 0; i<4; i++)
        {
            if (i < fixedKnifes.Length && fixedKnifes[i] != null)
            {
                Destroy(fixedKnifes[i]);
            }
            if (apples[i] != null)
            {
                Destroy(apples[i]);
            }            
        }
    }

    private void LookAt2D(GameObject obj, Transform pos)
    {
        Vector2 difference = obj.transform.position - pos.transform.position;

        float rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0, 0, rotation - 180);

    } //Rotaciona o obj para olhar para a pos, usado para rotacionar as facas e maçãs ao redor do target

    public int SetScore()
    {
        return knifesToComplete;
    }

}
