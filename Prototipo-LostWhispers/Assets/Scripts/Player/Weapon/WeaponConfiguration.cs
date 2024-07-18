using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;
using EZCameraShake;

public class WeaponConfiguration : MonoBehaviour
{
    //estadisticas de armas
    [Header("Daño del arma")]
    public int damage;
    [Header("El tiempo que pasa tras cada disparo")]
    public float timeBeetwenShooting;
    [Header("Dispersion de los disparos")]
    public float spread;
    [Header("El alcance de la bala")]
    public float range;
    [Header("El tiempo de recarga del arma")]
    public float reloadTime;
    [Header("El tiempo que pasa entre cada bala cuando hay multiples disparos")]
    public float timeBeetwenShots;
    [Header("Municion cargada al arma")]
    public int magazineSize;
    private int bulletPerTap = 1;//<---Por favor no tocar este valor
    [Header("Municion disponible")]
    public int totalAmmo;
    [Header("Indica si permite mantener presionado el boton para disparar continuamente")]
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //Booleanos
    bool shooting, readyToShoot, reloading;

    //Referencias
    [Header("Camara del jugador")]
    public Camera fpsCam;
    [Header("Objeto impactado")]
    public RaycastHit rayHit;
    [Header("Enemigo que se puede bajar la vida")]
    public LayerMask whatIsEnemy;

    //Camara shake
    [Header("Referencia a camaraShake")]
    public CameraShaker camShake;
    [Header("Temblar la camara")]
    public float camShakerMagnitude;
    [Header("Erratica de la camara")]
    public float roughness;
    [Header("El tiempo que tomara para alcanzar la magnitud completa de temblor de la camara")]
    public float fadeInTime;
    [Header("El tiempo que tarda para dejar de sacudir")]
    public float fadeOutTime;

    //Particulas
    [Header("Fogonazo del arma")]
    public GameObject muzzleEffect;
    [Header("Animacion de cartucho o bala")]
    public GameObject catridge;
    private ParticleSystem muzzleParticleSystem;
    private ParticleSystem catridgeParticleSystem;
    [Header("Humo al disparar")]
    [SerializeField] ParticleSystem smoke;
    /*Audio source que tomara los sonidos de forma aleatoria*/
    private AudioSource randomSource;
    [Header("Sonido de disparos")]
    public AudioClip[] audioShot;
    //Text que mostrara la informacion en pantalla
    [Header("Texto")]
    public Text ammoText;

    private void Awake()
    {
        muzzleParticleSystem = muzzleEffect.GetComponent<ParticleSystem>();
        catridgeParticleSystem = catridge.GetComponent<ParticleSystem>();
        randomSource = GetComponent<AudioSource>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        InputShot();
        ammoText.text = bulletsLeft + "/" + totalAmmo;
    }
    private void InputShot()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
        //Disparar
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;
        muzzleParticleSystem.Play();
        catridgeParticleSystem.Play();
        smoke.Play();
        RandomShot();
        //dispersion de bala
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calcular direccion de la dispersion
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            //Usar este if para el ucumar
            /*if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<ShootingAI>().TakeDamage(damage);
            }*/
        }
        /*CAMERA SHAKE*/
        CameraShaker.Instance.ShakeOnce(camShakerMagnitude, roughness, fadeInTime, fadeOutTime);
        /*Particulas*/
        //Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBeetwenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBeetwenShots);
            Debug.Log("Disparo!");
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        int bulletsToReload = magazineSize - bulletsLeft;
        if (totalAmmo >= bulletsToReload)
        {
            bulletsLeft += bulletsToReload;
            totalAmmo -= bulletsToReload;
        }
        else
        {
            bulletsLeft += totalAmmo;
            totalAmmo = 0;
            Debug.Log("No tienes municion");

        }
        reloading = false;
    }
    private void RandomShot()
    {
        randomSource.clip = audioShot[Random.Range(0, audioShot.Length)];
        randomSource.Play();
    }
}