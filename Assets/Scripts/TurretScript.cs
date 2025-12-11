using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretScript : MonoBehaviour
{

    SewerSceneManager _manager;
    public GameObject player;

    public LayerMask Layers;
    Vector2 aimTarget;
    public GameObject bulletPrefab;
    public float rotationSpeed;
    public float bulletSpeed;

    public float rateOfFire;
    float nextBulletLoaded = 0f;
    private bool shouldAim;
    private bool canFire;

    public GameObject turret;
    public Rigidbody2D turretRbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindAnyObjectByType<SewerSceneManager>();
        shouldAim = false;
        canFire = false;

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(shouldAim);
        Aim();
    }


    private float fireCooldown = 0f;
    private void FixedUpdate()
    {
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        if (canFire && fireCooldown <= 0f)
        {
            ShootBullet();
            fireCooldown = 1f / rateOfFire;
        }

    }

    void ShootBullet()
    {
        Debug.Log("PlayerImmune" + PlayerPrefs.GetInt("PlayerImmune"));
        if (PlayerPrefs.GetInt("PlayerImmune") == 1) return;

        if (Time.time < nextBulletLoaded) { return; }
        GameObject bullet = _manager.GetBullet();

        if (bullet == null) { return; }

        bullet.transform.rotation = Quaternion.identity;
        bullet.transform.position = turret.transform.position + Vector3.right;
        float rotation = turret.transform.rotation.eulerAngles.z;
        bullet.transform.RotateAround(turret.transform.position, Vector3.forward, rotation);
        Rigidbody2D BulletRbody = bullet.GetComponent<Rigidbody2D>();

        if (BulletRbody != null)
        {
            BulletRbody.linearVelocity = (bullet.transform.rotation) * Vector2.right * bulletSpeed;
        }

        nextBulletLoaded = Time.time + 1.0f / rateOfFire;
    }


    private float _time;
    public float angleRange;
    void Aim() {

        Vector2 direction = (Vector2)player.transform.position - turretRbody.position;
            Debug.DrawRay(
            turretRbody.position,
            direction.normalized * 500f,
            Color.red
        );
        
        RaycastHit2D hit = Physics2D.Raycast(
            turretRbody.position,
            direction.normalized,
            500f,
            Layers
        );

        if (hit.collider != null)
        {
            Debug.Log("Ray hit tag: " + hit.collider.tag);
        }
        else
        {
            Debug.Log("Ray hit NOTHING");
        }
        shouldAim = hit.collider != null && hit.collider.CompareTag("Player");

        if (shouldAim)
        {
            aimTarget = player.transform.position;
            float deltaRotationSpeen = rotationSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.AngleAxis(GetAngle(aimTarget), Vector3.forward);
            turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, rotation, deltaRotationSpeen);
            canFire = true;

        } else
        {
            canFire = false;
            _time += Time.deltaTime * 1;

            float angle = Mathf.Sin(_time) * angleRange;

            turret.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        }
    }


    public float GetAngle(Vector2 target)
    {
        Vector2 diff = target - turretRbody.position;
        return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    }

}