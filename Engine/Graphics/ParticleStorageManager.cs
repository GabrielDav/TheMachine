using System;
using System.Collections.Generic;

namespace Engine.Graphics
{
    public class ParticleStorageManager : IDisposable
    {
        public const int Maxparticle = 1000;

        public Dictionary<string, List<Particle>> ParticlesPool;

        public ParticleStorageManager()
        {
            ParticlesPool = new Dictionary<string, List<Particle>>();
        }

        public Particle GetParticle(BaseParticleManager manager, string particleType)
        {
            Particle newParticle;
            if (ParticlesPool.ContainsKey(particleType))
            {
                if (ParticlesPool[particleType].Count > 0)
                {
                    int particleCount = 0;

                    foreach (var particle in ParticlesPool[particleType])
                    {
                        if (!particle.Active)
                        {
                            return particle;
                        }

                        particleCount++;
                    }

                    if (particleCount >= Maxparticle)
                    {
                        throw new Exception("Too much particles!");
                    }
                    newParticle = manager.CreateNewParticle();
                    ParticlesPool[particleType].Add(newParticle);

                    return newParticle;
                }

                newParticle = manager.CreateNewParticle();
                ParticlesPool[particleType].Add(newParticle);

                return newParticle;
            }

            ParticlesPool.Add(particleType, new List<Particle>());
            newParticle = manager.CreateNewParticle();
            ParticlesPool[particleType].Add(newParticle);

            return newParticle;
        }

        public void Update()
        {
            foreach (var particles in ParticlesPool)
            {
                foreach (var particle in particles.Value)
                {
                    if (particle.Active)
                    {
                        particle.Update();
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var particleList in ParticlesPool.Values)
            {
                particleList.Clear();
            }
            ParticlesPool.Clear();
        }
    }
}
