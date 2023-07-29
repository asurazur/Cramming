using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera sprintVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera FPSVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    [SerializeField] private GameObject cameraTPSFollow;
    [SerializeField] private GameObject cameraFPSFollow;
    [SerializeField] private GameObject bodyAnimDirectionFollow;

    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;




    [SerializeField] private float bulletsPerSecond; // Set this in the Inspector
    private bool canShoot = true;
    private float shootTimer;


    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsinputs;
    private Animator animator;

    private bool is_fps = false;
    private bool reloading = false;
    private float reloadStartTime;
    private float reloadDuration = 3f; // 2 seconds

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsinputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();

        currentAmmo = maxAmmo;
    }
     
    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Transform hitTransform = null;

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)){
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;

            hitTransform = raycastHit.transform;
        }


        // Reload

        if (starterAssetsinputs.reload && !reloading)
        {
            StartReloading();
        }

        if (reloading && Time.time >= reloadStartTime + reloadDuration)
        {
            FinishReloading();
        }

        // FPS TOGGLE
        if (starterAssetsinputs.fps) 
        {
            is_fps = !is_fps;
            starterAssetsinputs.fps = false;


        }
        // AIMING TPS 
        if (is_fps != true)
        {
            if (starterAssetsinputs.aim)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                thirdPersonController.SetRotateOnMove(false);

                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

                // player rotates into the direction of the player
                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;  
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            } 
            else if (!starterAssetsinputs.aim && starterAssetsinputs.sprint) 
            {
                sprintVirtualCamera.gameObject.SetActive(true);
                aimVirtualCamera.gameObject.SetActive(false);
                thirdPersonController.SetRotateOnMove(true);


            }
            else
            {
                aimVirtualCamera.gameObject.SetActive(false);
                sprintVirtualCamera.gameObject.SetActive(false);
                FPSVirtualCamera.gameObject.SetActive(false);
                thirdPersonController.CinemachineCameraTarget = cameraTPSFollow;

                thirdPersonController.SetSensitivity(normalSensitivity);
                thirdPersonController.SetRotateOnMove(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            }
        }
        else
        {
            if (starterAssetsinputs.aim)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            }
            else
            {
                aimVirtualCamera.gameObject.SetActive(false);
                sprintVirtualCamera.gameObject.SetActive(false);
                FPSVirtualCamera.gameObject.SetActive(true);
                thirdPersonController.CinemachineCameraTarget = cameraFPSFollow;

                // player rotates into the direction of the player
                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;  
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }

        }
        
        // SHOOT    

        if (starterAssetsinputs.shoot && !reloading)
        {
            // make model face direction
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;  
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 32);

        if (starterAssetsinputs.shoot && canShoot && currentAmmo != 0) {

            currentAmmo -= 1;

            Debug.Log("Ammo Count:");
            Debug.Log(currentAmmo);



            thirdPersonController.GetAnimator().Play("Gunplay");
        
        Debug.Log(Time.deltaTime * 10f);

        //// projectile
        // Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        // Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        // starterAssetsinputs.shoot = false;


        // hitscan
            if (hitTransform != null) {
                if (hitTransform.GetComponent<BulletTarget>() != null) {
                    Instantiate(vfxHitGreen, debugTransform.position, Quaternion.identity);
                }
                else {
                    Instantiate(vfxHitRed, debugTransform.position, Quaternion.identity);
                } 
            }

            //starterAssetsinputs.shoot = false;
            thirdPersonController.GetAnimator().SetBool(thirdPersonController.GetAnimIDShoot(), false);

            shootTimer = 1f / bulletsPerSecond;
            canShoot = false;
        }




        }



        if (!canShoot)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                canShoot = true;
            }
        }



        
    }

    private void StartReloading()
    {
        reloading = true;
        reloadStartTime = Time.time;

        // Play the reloading animation here
        thirdPersonController.GetAnimator().Play("Reloading");
    }

    private void FinishReloading()
    {
        reloading = false;

        // Set your currentAmmo to maxAmmo here
        currentAmmo = maxAmmo;

        // Reset the reload input so that it doesn't trigger the reloading again
        starterAssetsinputs.reload = false;
    }


}
