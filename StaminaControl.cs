using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaControl : MonoBehaviour
{
    public Image staminaUI;
    public float staminaDuration = 5f;
    public float currentStamina;
    public GameObject player;
    private bool canRun = true;

    // Start is called before the first frame update
    void Start()
    {
        staminaUI.fillAmount = 1f;
        currentStamina = staminaDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Movement>().running && canRun && player.GetComponent<Movement>().direction != Vector2.zero)
        {
            currentStamina -= Time.deltaTime; // Decaimento do timer
            staminaUI.fillAmount = currentStamina / staminaDuration; // Atualiza a barra em porcentagem com base no tempo atual/total
            player.GetComponent<Movement>().speed = 6;
        }
        else if (!player.GetComponent<Movement>().running && currentStamina <= staminaDuration)
        {
            currentStamina += Time.deltaTime / 5; // Decaimento do timer
            staminaUI.fillAmount = currentStamina / staminaDuration; // Atualiza a barra em porcentagem com base no tempo atual/total
            player.GetComponent<Movement>().speed = 2;
        }

        if (currentStamina <= 0f) // Se stamina acabou, volta a velocidade normal e impossibilita correr
        {
            canRun = false;
            player.GetComponent<Movement>().speed = 2;
        }
        else if (currentStamina > 0f) // Se a barra não está vazia, é possível correr, mesmo sem estar cheia
        {
            canRun = true;
        }
    }
}
