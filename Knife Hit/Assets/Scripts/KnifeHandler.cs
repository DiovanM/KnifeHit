using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KnifeHandler : MonoBehaviour {

    [SerializeField]
    private GameObject knifePrefab;
    [SerializeField]
    private GameObject firstKnife;

    private GameObject[] knifes = new GameObject[10];
    private int knifeIndex = 1;

    private float time;

    private void Awake()
    {
        time = 0;
        KnifeBehaviour.Hit += NextKnife;
        GameManager.ResetKnifesParameters += RemoveParentAndDeactivate;
        GameManager.ResetKnife += ResetActualKnife;
        GameManager.NextKnife += NextKnife;
        knifeIndex = 1;
    }

    private void OnDestroy()
    {
        KnifeBehaviour.Hit -= NextKnife;
        GameManager.ResetKnifesParameters -= NextKnife;
        GameManager.ResetKnifesParameters -= RemoveParentAndDeactivate;
    }

    private void Start()
    {
        knifes[0] = firstKnife; //Define a faca inicial como a posição 0 do array para o pool;
        InstantiateKnifes();
    }

    private void Update()
    {
        if (time < 1)
        {
            ChangeFirstKnifePosition();
        } else if (time < 2)
        {
            firstKnife.GetComponent<KnifeBehaviour>().enabled = true;
            time = 3;
        }
    }

    void ChangeFirstKnifePosition() //Move a faca inicial para a posição padrão
    {
        time += Time.deltaTime * 2;
        firstKnife.transform.position = Vector3.Lerp(firstKnife.transform.position, transform.position, time);
    }

    void InstantiateKnifes() //Instancia as facas para o pool em um array
    {
        for (int i = 1; i < 10; i++)
        {
            knifes[i] = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        }
    }

    void NextKnife() //Troca para a próxima faca no pool
    {
        knifes[knifeIndex].transform.SetParent(null);
        knifes[knifeIndex].transform.position = transform.position;
        knifes[knifeIndex].transform.rotation = transform.rotation;
        knifes[knifeIndex].gameObject.SetActive(true);
        knifes[knifeIndex].GetComponent<KnifeBehaviour>().enabled = true;
        knifeIndex++;
        if (knifeIndex > 9) {
            knifeIndex = 0;
        }
    }

    void RemoveParentAndDeactivate() //Usado ao reiniciar o jogo para desativar as facas e removê-las do parent
    {
        for (int i = 0; i < 10; i++)
        {
            knifes[i].transform.SetParent(null);
            knifes[i].SetActive(false);
        }
        knifeIndex = 0;
    }

    void ResetActualKnife()
    {
        knifes[knifeIndex - 1].transform.position = transform.position;
        knifes[knifeIndex - 1].transform.rotation = transform.rotation;
        knifes[knifeIndex - 1].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        knifes[knifeIndex - 1].SetActive(false);
    }

    void ResetAllKnifes()
    {
        for (int i = 0; i < 10; i++)
        {
            knifes[i].transform.SetParent(null);
            knifes[i].SetActive(false);
        }
    }

}
