# 🚀 ENRUTA - Endless Runner Cívico
**Desarrollado por [Nombre del Estudio, ej: PIXEL CÍVICO]**



## 🛠️ FLUJO DE TRABAJO DEL EQUIPO (Proceso Estandarizado)

### ⚙️ ESTRUCTURA DE RAMAS (Flujo de Trabajo)

| Flujo | Acción Principal | Herramienta Clave | Propósito del Paso |
| :--- | :--- | :--- | :--- |
| **Inicio Diario (Sincronización)** | Hacer **Pull/Fetch** desde la rama **`dev`** a la rama local **`dev`**. | Git Desktop | Asegurar que el desarrollador tiene la versión más reciente del trabajo del equipo. |
| **Desarrollo Diario** | Crear cambios, hacer **Commit** con mensajes descriptivos. | Git Desktop | Registrar el progreso local en la rama personal (`wilson`, etc.). |
| **Subir Cambios** | Hacer **Push** de la rama personal a GitHub. | Git Desktop | Compartir el código con el repositorio remoto, preparándose para el *merge*. |
| **Integración (PR)** | Crear el **Pull Request (PR)** de la rama personal a **`dev`**. | GitHub.com | Iniciar la revisión de código y la prueba de integración. |
| **Resolución de Conflictos** | Abrir archivos en conflicto y resolver manualmente las líneas de código. | Visual Studio Code | Herramienta esencial para resolver conflictos de *merge* de manera eficiente y precisa. |
| **Merge Final** | Aprobar y completar el *Merge* del PR a la rama **`dev`**. | GitHub.com | Integrar código estable y aprobado al tronco de desarrollo activo. |
| **Hotfix (Crítico)** | **Bifurcar de `main`** $\to$ Arreglar $\to$ **Merge a `main` y `dev`**. | GitHub.com / Git Desktop | Proceso de emergencia para parchar *bugs* en la versión de producción sin desestabilizar `dev`. |



### ⚙️ ESTRUCTURA DE RAMAS (Flujo de Trabajo)

| Rama | Propósito Principal | Regla de Bifurcación y Merge |
| :--- | :--- | :--- |
| **`main`** | **PRODUCCIÓN (Versión Estable)**. Lo que ve el publico final. | Solo *merge* desde `dev` o `hotfix`. |
| **`dev`** | **INTEGRACIÓN DIARIA.** Versión de *testing* activa. | *Merge* desde ramas secundarias (`wilson`, `art-assets`, etc.). |
| **`hotfix`** | **CORRECCIONES CRÍTICAS EN PRODUCCIÓN.** Para *bugs* urgentes en `main`. | **Bifurca de `main`. Mergea a `main` Y `dev`.** |
| **`wilson`, `juan-sebastian`** | TRABAJO CORE/LÓGICA. | *Merge* a `dev` vía Pull Request (PR). |
| **`ui-ux`** | INTERFACES Y LAYOUTS. | *Merge* a `dev` vía PR. |
| **`art-assets`** | MODELOS Y ESCALA 3D. | *Merge* a `dev` vía PR. |

### 🛑 REGLA CLAVE: PULL REQUESTS (PR)

**NUNCA HACER *PUSH* DIRECTO A `main` O `dev` DESDE UNA RAMA PERSONAL O DE EQUIPO.**

Todas las integraciones a `dev` deben hacerse a través de un **Pull Request (PR)** y deben ser aprobadas por el Líder de Proyecto/Core.

---

### 💻 REQUERIMIENTOS TÉCNICOS
* **Motor de Juego:** Unity (6000.0.48f1).
* **Herramienta de Merge Recomendada:** **Visual Studio Code** (para la resolución manual de conflictos).

🛠️ FLUJO DE TRABAJO DEL EQUIPO (Git Desktop & VS Code)
Este es el proceso estandarizado que el equipo debe seguir para el control de versiones, utilizando Git Desktop para el manejo diario y Visual Studio Code para la resolución de conflictos.

A. Flujo Diario de Desarrollo
Este proceso se realiza principalmente con Git Desktop para los commits y push rápidos.

Sincronizar dev: Al iniciar la jornada, cambia a la rama dev y haz Pull/Fetch para tener los últimos cambios integrados por el equipo.

Trabajar: Cambia a tu Rama Personal (wilson, juan-sebastian, etc.).

Comprometer: Haz cambios y crea un Commit descriptivo en Git Desktop.

Subir: Haz Push a tu rama personal en GitHub.

B. Flujo de Integración (Pull Request - PR)
Este proceso es la Regla de Oro y asegura la calidad del código.

Crear el PR: Cuando una tarea esté finalizada, ve a GitHub.com y crea un Pull Request de tu rama-personal a dev.

Notificar: Anuncia en el canal de Discord #control-de-versiones que el PR está listo para revisión.

Resolución de Conflictos (Visual Studio Code):

Si hay conflictos, el desarrollador responsable abre los archivos conflictivos en Visual Studio Code.

Resuelve manualmente los conflictos línea por línea.

Una vez resuelto, hace un nuevo Commit y Push a la rama personal para actualizar el PR.

Merge Final: El Líder del Proyecto aprueba y completa el Merge a dev.

C. Flujo de Hotfix (Corrección Crítica)
Este flujo se usa solo para solucionar bugs críticos en la versión de main (Producción).

Bifurcación: El Programador Core crea una nueva rama hotfix/nombre-del-bug desde main.

Arreglo: Aplica el arreglo crítico en esta rama.

Doble Merge: Una vez verificado, el Líder de Proyecto realiza el merge a main (producción) y luego replica el merge a dev para asegurar que el arreglo no se pierda.
