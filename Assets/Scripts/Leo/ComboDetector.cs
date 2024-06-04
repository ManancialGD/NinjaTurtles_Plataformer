using UnityEngine;

public class ComboDetector : MonoBehaviour
{

    float timeout = 1f; //Tempo de espera até atacar novamente
    int comboIndex = 0; //Registo de combo atual (informação que pode ser útil para escolher as animações)
    float comboTime = 0.75f; //Tempo perfeito para o combo
    float timeLimit = 0.1f; //O tempo em que o combo pode ser detetado

    void Update()
    {
        if (timeout > 0) timeout -= Time.deltaTime;
        if (timeout < 0) timeout = 0; // para não a variavel não estar a atualizar desnecessariamente
    }

    public void OnPlayerAttack()
    {
        if (isComboTiming())
        {
            if (comboIndex >= 2) // ao 3º combo proibe de continuar
            {
                comboIndex = 0;
                // Ataque combo 3 (final)
                InitializeCooldown(2f);
            }
            else
            {
                comboIndex++;
                // Ataque combo 1 ou 2
                InitializeCooldown(1f);
            }
        }
        else
        {
            if (canAttack())
            {
                // Ataque normal
                InitializeCooldown(1f);
            }
            else
            {
                // Ainda não consegue atacar
                return;
            }
        };
    }

    // Verifica se comboTime coincide com o tempo desde o último ataque (dentro do limite de imprecisão timeLimit)
    public bool isComboTiming() => timeout > comboTime - timeLimit / 2 && timeout < comboTime + timeLimit / 2;
    public bool canAttack() => timeout <= 0;
    public void InitializeCooldown(float time) { timeout = time; }
}