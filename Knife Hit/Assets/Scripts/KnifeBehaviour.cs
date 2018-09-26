using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : MonoBehaviour {

    public delegate void HitTarget();
    public static event HitTarget Hit;

    public delegate void Lose();
    public static event Lose HitKnife;

    public delegate void AppleCounter();
    public static event AppleCounter AppleUp;

    [SerializeField]
    private float shootingForce;

    private bool canShoot;
    private bool hasHit;

    private void Awake()
    {
        hasHit = false;
    }

    private void OnEnable()
    {
        canShoot = true;
        hasHit = false;
        if (!GetComponent<Rigidbody2D>()) //Readiciona o rigidbody desativado no trigger
        {
            gameObject.AddComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    void Update () {
        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
                canShoot = false;
            }
        }
	}

    public void Shoot()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * shootingForce * 10);
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Knife") && !hasHit) //Ao colidir com outra faca, chama o event HitKnife e
        {                                                        //adiciona forças na direção contrária e aleatoriamente
                                                                 //para a direita ou esquerda
            if (GetComponent<Rigidbody2D>())
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                GetComponent<Rigidbody2D>().AddForce(Vector2.down * shootingForce * 5);
                if (Random.Range(0, 2) == 0)
                {
                    GetComponent<Rigidbody2D>().angularVelocity = (shootingForce * 10);
                    GetComponent<Rigidbody2D>().AddForce(Vector2.left * shootingForce * 5);
                }
                else
                {
                    GetComponent<Rigidbody2D>().angularVelocity = (shootingForce * -10);
                    GetComponent<Rigidbody2D>().AddForce(Vector2.right * shootingForce * 5);
                }
                HitKnife();
                GetComponent<KnifeBehaviour>().enabled = false;
                hasHit = true;
            }
        }
        if (collision.gameObject.CompareTag("Target") && !hasHit) //"Prende" a faca no alvo e chama o event que instancia a próxima faca
        {            
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Destroy(GetComponent<Rigidbody2D>());
            transform.SetParent(collision.gameObject.transform);
            Hit();
            GetComponent<KnifeBehaviour>().enabled = false;
            hasHit = true;
        }

        if (collision.gameObject.CompareTag("Apple"))
        {
            Destroy(collision.gameObject);
            AppleUp();
        }

    }   

}
