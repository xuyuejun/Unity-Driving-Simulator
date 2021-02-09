using UnityEngine;
using System.Collections;

public class ParticleGenerator : MonoBehaviour
{
    public bool isRaining;                              // Do I have to spwan water drops?
    public Rigidbody carbody;                           // This needs the rigidBody of the car for passing it to the "DynamicParticle" class to simulate the movement of the waterdrops by the G-Force of the car
    public int maxSpawn;                                // Maximum waterdrops that this specific waterdrop generator can make
    public int PARTICLE_LIFETIME_MustBeMoreThanOne = 3; // How much time will each particle live
    public Transform particlesParent;                   // Where will the spawned particles will be parented (To avoid covering the whole inspector with them)
    [HideInInspector]
    public int k = -99;                                 // This indicates if a new waterdrop can be spwaned or a waterdrop is deleted, see the "Update()" and "SpawnNow()" functions of this class for more info.

    void LateUpdate()
    {
        if (k != (-99))
            if (k < maxSpawn && isRaining)
                StartCoroutine(SpawnNew());
    }

    IEnumerator SpawnNew()
    {
        GameObject newLiquidParticle = (GameObject)Instantiate(Resources.Load("WFX/WaterDrop")); //Spawn a particle prefab (located in the Resource folder)
        k++;                                           // Indicate that a new waterdrop particle is added to the scene
        DynamicParticle particleScript = newLiquidParticle.GetComponent<DynamicParticle>(); // Get the script of the waterdrop particle
        particleScript.OR = carbody;                   // Refrence the rigidBody of the car to the script for the G-Force calculations
        particleScript.SetLifeTime(Random.Range(1f, PARTICLE_LIFETIME_MustBeMoreThanOne)); //Set each particle lifetime
        particleScript.Spawner = this;
        newLiquidParticle.transform.position = transform.position;// Relocate the waterdrop to the spawner position
        newLiquidParticle.transform.parent = particlesParent;// Add the particle to the parent container			
        yield return new WaitForSeconds(Random.Range(0.2f, 5f)); //We don't want to spawn all the waterdrops at once, we spawn them every 0.2 to 5 seconds to mimic the rain effect in real life.
    }
}