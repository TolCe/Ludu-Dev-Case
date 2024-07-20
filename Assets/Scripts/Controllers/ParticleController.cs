using System.Collections;
using UnityEngine;

public class ParticleController : Singleton<ParticleController>
{
    [SerializeField] private ParticleSystem _shatterParticlePrefab;
    [SerializeField] private Transform _particleContainer;
    private ObjectPool<ParticleSystem> _shatterParticlePool;

    protected override void Awake()
    {
        base.Awake();

        CreatePool();
    }

    private void CreatePool()
    {
        _shatterParticlePool = new ObjectPool<ParticleSystem>(_shatterParticlePrefab, 5, _particleContainer);
    }

    public void PlayParticleAtPosition(Vector3 position)
    {
        ParticleSystem particle = _shatterParticlePool.Get();
        particle.transform.position = position;
        particle.gameObject.SetActive(true);
        particle.Play();
        StartCoroutine(WaitForParticle(particle));
    }

    private IEnumerator WaitForParticle(ParticleSystem particle)
    {
        yield return new WaitForSeconds(2f);
        _shatterParticlePool.Return(particle);
    }
}
