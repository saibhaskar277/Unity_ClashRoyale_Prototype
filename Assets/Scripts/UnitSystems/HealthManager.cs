using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] float health = 100f;

    float currentHealth;
    float maxHealth;

    [SerializeField] Slider healthSlider;

    public Transform Target => transform;

    public UnitCategory Category => category;

    [SerializeField]UnitCategory category; 

    private void Awake()
    {
        maxHealth = health;
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
    }

    private void LateUpdate()
    {
        if (healthSlider == null || Camera.main == null)
            return;

        healthSlider.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void Configure(float healthValue)
    {
        maxHealth = healthValue;
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.InverseLerp(0, maxHealth, currentHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        UnitPoolMember poolMember = GetComponent<UnitPoolMember>();
        if (poolMember != null)
        {
            UnitPoolManager.Instance.Despawn(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}