using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public GameObject[] hearts;

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // garante que coração está ativo
                if (!hearts[i].activeSelf)
                    hearts[i].SetActive(true);
            }
            else
            {
                // só aplica efeito se ainda estiver ativo
                if (hearts[i].activeSelf)
                {
                    var effect = hearts[i].GetComponent<HeartEffect>();

                    if (effect != null)
                    {
                        effect.PlayLoseEffect(); // 🔥 efeito bonito
                    }
                    else
                    {
                        hearts[i].SetActive(false); // fallback
                    }
                }
            }
        }
    }
}