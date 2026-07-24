using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum BodyPart { Head, Torso, Arm, Leg }

    [SerializeField] private BodyPart part = BodyPart.Torso;
    [SerializeField] private int score = 10;
    [SerializeField] private int damage = 20;
    [SerializeField] private ZombieHealth health;

    public BodyPart Part { get { return part; } }
    public int Score { get { return score; } }

    public void Configure(BodyPart bodyPart, int scoreValue, int damageValue, ZombieHealth target)
    {
        part = bodyPart;
        score = scoreValue;
        damage = damageValue;
        health = target;
    }

    public int RegisterHit()
    {
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        return score;
    }

    public string PartLabel()
    {
        switch (part)
        {
            case BodyPart.Head: return "Cabeza";
            case BodyPart.Arm: return "Brazo";
            case BodyPart.Leg: return "Pierna";
            default: return "Torso";
        }
    }
}
