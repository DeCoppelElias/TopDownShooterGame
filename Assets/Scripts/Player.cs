using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public float moveSpeed = 3;
    public float moveSpeedShooting = 1.5f;
    public float accelerationSpeed = 1;
    public float currentSpeed = 0;
    public float attackSpeed = 1.5f;
    public float level = 0;

    public float reflectShieldCooldown = 3;
    private float reflectTime = 0;

    public Canvas canvas;
    public GameObject disabledUI;
    public List<Image> activeUI;

    public Image firstUpgradePrefab;
    public Image sniperUpgradePrefab;
    public Image statsUpgradePrefab;
    public Image bulletUpgradePrefab;
    public Image gunnerUpgradePrefab;
    public Image heavyUpgradePrefab;

    private Image firstUpgrade;
    private Image sniperUpgrade;
    private Image statsUpgrade;
    private Image bulletUpgrade;
    private Image gunnerUpgrade;
    private Image heavyUpgrade;

    private string currentClass = "Basic";

    public GameObject bulletWall;

    private float frameCounter = 0;
    private float lastShot = 0;


    //STORYLINE: EERST ALLES IN GRIJSTINTEN
    //BIJ HET VERSLAAN VAN EEN BAAS WORDT ER IETS INGEKLEURD

    public override void StartEntity()
    {
        firstUpgrade = CreateFirstUpgrade();
        sniperUpgrade = CreateSniperUpgrade();
        statsUpgrade = CreateUpgradeStats();
        bulletUpgrade = CreateUpgradeBullet();
        gunnerUpgrade = CreateGunnerUpgrade();
        heavyUpgrade = CreateHeavyUpgrade();
    }

    public override void UpdateEntity()
    {
        frameCounter++;
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > reflectTime + reflectShieldCooldown)
        {
            GetComponent<ReflectShieldAbility>().CreateReflectShield();
            reflectTime = Time.time;
        }
        float attackSpeed = gameObject.GetComponent<Player>().attackSpeed;
        if (Input.GetButton("Fire1") && Time.time > lastShot + 1 / attackSpeed && Time.timeScale > 0)
        {
            GetComponent<ShootingAbility>().Shoot();
            lastShot = Time.time;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy && frameCounter > 500)
        {
            health -= enemy.damage;
            frameCounter = 0;
        }
    }

    public void LevelUp()
    {
        Time.timeScale = 0;
        EnableUI(statsUpgrade);
    }
    public void ClassUp()
    {
        Time.timeScale = 0;
        if (currentClass == "Basic")
        {
            EnableUI(firstUpgrade);
        }
        else if (currentClass == "Sniper")
        {
            EnableUI(sniperUpgrade);
        }
        else if (currentClass == "Gunner")
        {
            EnableUI(gunnerUpgrade);
        }
        else if (currentClass == "Heavy")
        {
            EnableUI(heavyUpgrade);
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public Image CreateHeavyUpgrade()
    {
        Image currentUI = Instantiate(heavyUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => MakeDashingHeavy(currentUI));
        buttons[1].onClick.AddListener(() => MakeShotgunHeavy(currentUI));
        buttons[2].onClick.AddListener(() => MakeNukeHeavy(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void MakeDashingHeavy(Image currentUI)
    {
        GetComponent<PlayerMovement>().dashCooldown /= 2;
        currentClass = "DashingHeavy";
        DisableUI(currentUI);
    }
    public void MakeShotgunHeavy(Image currentUI)
    {
        GetComponent<ShootingAbility>().totalFan += 2;
        GetComponent<ShootingAbility>().totalSplit += 1;
        GetComponent<ShootingAbility>().range -= 1;
        currentClass = "ShotgunHeavy";
        DisableUI(currentUI);
    }
    public void MakeNukeHeavy(Image currentUI)
    {
        damage += 25;
        GetComponent<ShootingAbility>().bulletSize = 1.5f;
        currentClass = "NukeHeavy";
        DisableUI(currentUI);
    }

    public Image CreateGunnerUpgrade()
    {
        Image currentUI = Instantiate(gunnerUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => MakeDoubleGunner(currentUI));
        buttons[1].onClick.AddListener(() => MakeSplitGunner(currentUI));
        buttons[2].onClick.AddListener(() => MakeCloseRangeGunner(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void MakeDoubleGunner(Image currentUI)
    {
        GetComponent<ShootingAbility>().totalSplit += 1;
        currentClass = "DoubleGunner";
        DisableUI(currentUI);
    }
    public void MakeSplitGunner(Image currentUI)
    {
        GetComponent<ShootingAbility>().totalFan += 2;
        currentClass = "SplitGunner";
        DisableUI(currentUI);
    }
    public void MakeCloseRangeGunner(Image currentUI)
    {
        attackSpeed += 0.5f;
        GetComponent<ShootingAbility>().range -= 1;
        currentClass = "CloseRangeGunner";
        DisableUI(currentUI);
    }

    public Image CreateFirstUpgrade()
    {
        Image currentUI = Instantiate(firstUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => MakeSniper(currentUI));
        buttons[1].onClick.AddListener(() => MakeGunner(currentUI));
        buttons[2].onClick.AddListener(() => MakeHeavy(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void MakeSniper(Image currentUI)
    {
        damage += 25;
        GetComponent<ShootingAbility>().range = 6;
        attackSpeed -= -1.25f;
        moveSpeed += 1;
        currentClass = "Sniper";
        DisableUI(currentUI);
    }
    public void MakeGunner(Image currentUI)
    {
        damage += 10;
        attackSpeed += 0.5f;
        currentClass = "Gunner";
        DisableUI(currentUI);
    }
    public void MakeHeavy(Image currentUI)
    {
        damage += 50;
        attackSpeed -= 1;
        moveSpeed -= 0.5f;
        GetComponent<ShootingAbility>().bulletSize = 1f;
        currentClass = "Heavy";
        DisableUI(currentUI);
    }
    
    public Image CreateSniperUpgrade()
    {
        Image currentUI = Instantiate(sniperUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => MakeCollateralSniper(currentUI));
        buttons[1].onClick.AddListener(() => MakeHeadshotSniper(currentUI));
        buttons[2].onClick.AddListener(() => MakeDodgeSniper(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void MakeCollateralSniper(Image currentUI)
    {
        GetComponent<ShootingAbility>().pierce += 2;
        currentClass = "CollateralSniper";
        DisableUI(currentUI);
    }
    public void MakeHeadshotSniper(Image currentUI)
    {
        damage += 50;
        currentClass = "HeadshotSniper";
        DisableUI(currentUI);
    }
    public void MakeDodgeSniper(Image currentUI)
    {
        GetComponent<PlayerMovement>().dashCooldown /= 2;
        currentClass = "DodgeSniper";
        DisableUI(currentUI);
    }

    public Image CreateUpgradeStats()
    {
        Image currentUI = Instantiate(statsUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => IncreaseMovementSpeed(currentUI));
        buttons[1].onClick.AddListener(() => IncreaseDamage(currentUI));
        buttons[2].onClick.AddListener(() => IncreaseAttackSpeed(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void IncreaseDamage(Image currentUI)
    {
        damage += 10;
        DisableUI(currentUI);
    }
    public void IncreaseAttackSpeed(Image currentUI)
    {
        attackSpeed += 0.5f;
        DisableUI(currentUI);
    }
    public void IncreaseMovementSpeed(Image currentUI)
    {
        moveSpeed += 0.5f;
        DisableUI(currentUI);
    }

    public Image CreateUpgradeBullet()
    {
        Image currentUI = Instantiate(bulletUpgradePrefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => IncreasePierce(currentUI));
        buttons[1].onClick.AddListener(() => IncreaseSplit(currentUI));
        buttons[2].onClick.AddListener(() => IncreaseRange(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void IncreaseSplit(Image currentUI)
    {
        GetComponent<ShootingAbility>().totalSplit += 1;
        DisableUI(currentUI);
    }
    public void IncreasePierce(Image currentUI)
    {
        GetComponent<ShootingAbility>().pierce += 1;
        DisableUI(currentUI);
    }
    public void IncreaseRange(Image currentUI)
    {
        GetComponent<ShootingAbility>().range += 0.2f;
        DisableUI(currentUI);
    }


    public void DisableUI(Image currentUI)
    {
        activeUI.Remove(currentUI);
        currentUI.transform.SetParent(disabledUI.transform);
        if (activeUI.Count == 0)
        {
            Time.timeScale = 1;
        }
    }
    public void EnableUI(Image currentUI)
    {
        activeUI.Add(currentUI);
        currentUI.transform.SetParent(canvas.transform);
        if (activeUI.Count == 0)
        {
            Time.timeScale = 1;
        }
    }
}
