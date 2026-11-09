using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int startHealth = 50;
    [SerializeField] private TextMeshProUGUI healthText;

    private int currentHealth;

    // Start is called once before the first execution of Update
    void Start()
    {
        currentHealth = Mathf.Clamp(startHealth, 0, maxHealth);
        UpdateHealthText();
    }

    // Update is called once per frame

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        healthText.text = "HP " + currentHealth + "/" + maxHealth;
    }
}