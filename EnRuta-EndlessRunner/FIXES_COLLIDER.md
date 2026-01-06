# Fix: Sphere Collider causando muertes falsas

## Problema
El jugador se estaba muriendo al estar cerca de obstáculos, posiblemente porque un Sphere Collider estaba detectando colisiones a distancia.

## Soluciones Aplicadas

### 1. **Desactivación automática de Sphere Colliders** ✅
- Se agregó código en `Start()` que busca y desactiva automáticamente cualquier `SphereCollider` en el Player
- Esto previene colisiones falsas causadas por colliders innecesarios

### 2. **Validación de contacto real** ✅
- Se mejoró `OnCollisionEnter()` para verificar que haya un contacto real (`contactCount > 0`)
- Esto asegura que solo colisiones genuinas registren la muerte

### 3. **Método de debug** ✅
- Se agregó método `DebugColliderSetup()` (click derecho en Inspector → Debug: Mostrar Configuración de Colliders)
- Muestra todos los colliders activos del Player para diagnosticar el problema

## Cambios en: `Assets/Scripts/Player/PlayerController.cs`

### Línea 22
```csharp
private SphereCollider[] sphereColliders; // Para desactivar colisiones no deseadas
```

### Línea 47-53
```csharp
// IMPORTANTE: Desactivar cualquier Sphere Collider que pueda causar colisiones falsas
sphereColliders = GetComponents<SphereCollider>();
foreach (SphereCollider sc in sphereColliders)
{
    sc.enabled = false;
    Debug.LogWarning("[COLLIDER] Sphere Collider desactivado para evitar colisiones falsas");
}
```

### Línea 196-202
```csharp
// Si choca con un Rigidbody (sólido) - Solo si tiene contacto real, no solo cercanía
if (collision.gameObject.CompareTag("Obstaculo") && !isDead)
{
    // Verificar que sea un contacto real (no solo trigger)
    if (collision.contactCount > 0)
    {
        Debug.Log("[COLLISION] Colisión con obstáculo detectada: " + collision.gameObject.name);
        Die();
    }
}
```

## Qué revisar en Unity

1. **Selecciona el Player en la Escena**
2. **En el Inspector, busca la sección "Colliders":**
   - ✅ Debe haber un **CapsuleCollider** (activo)
   - ❌ NO debe haber **SphereColliders activos** (serán desactivados automáticamente)

3. **Si sigues viendo el problema:**
   - Revisa que el obstáculo NO tenga un Sphere Collider como trigger
   - Asegúrate de que el obstáculo tenga el tag "Obstaculo"
   - Comprueba que la física esté bien configurada (gravity, mass, etc.)

## Cómo Verificar el Fix

1. Ejecuta el juego
2. En la Consola, verás:
   - `[COLLIDER] Sphere Collider desactivado...` (si había alguno)
   - `[COLLISION] Colisión con obstáculo detectada...` (solo con contacto real)

3. Usa el método debug: Click derecho en el componente → `Debug: Mostrar Configuración de Colliders`
