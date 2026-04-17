<script setup lang="ts">
// #визард_проекта / #project_wizard_view
// Multi-step wizard for creating a project (5 steps)
// Многошаговый визард для создания проекта (5 шагов)
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import WizardStep1 from '@/src/components/wizard/WizardStep1.vue'
import WizardStep2 from '@/src/components/wizard/WizardStep2.vue'
import WizardStep3 from '@/src/components/wizard/WizardStep3.vue'
import WizardStep4 from '@/src/components/wizard/WizardStep4.vue'
import WizardStep5 from '@/src/components/wizard/WizardStep5.vue'

const router        = useRouter()
const projectsStore = useProjectsStore()

const currentStep = ref(1)
const totalSteps  = 5

// Wizard data collected across steps / Данные собираемые по шагам
const formData = ref({
  name:            '',
  startDate:       '',
  endDate:         '',
  priority:        1,
  customerCompany: '',
  executorCompany: '',
  projectManagerId: '',
  employeeIds:     [] as string[],
  documents:       [] as File[],
})

function nextStep() {
  if (currentStep.value < totalSteps) currentStep.value++
}

function prevStep() {
  if (currentStep.value > 1) currentStep.value--
}

function validateBeforeSubmit(): string | null {
  const dto = formData.value

  if (!dto.name.trim()) return 'Project name is required.'
  if (!dto.customerCompany.trim()) return 'Customer company is required.'
  if (!dto.executorCompany.trim()) return 'Executor company is required.'
  if (!dto.startDate) return 'Start date is required.'
  if (!dto.endDate) return 'End date is required.'
  if (!dto.projectManagerId) return 'Project manager is required.'
  if (dto.priority < 1) return 'Priority must be at least 1.'
  if (new Date(dto.startDate) > new Date(dto.endDate)) return 'Start date cannot be later than end date.'

  return null
}

async function submitWizard() {
  try {
    const validationError = validateBeforeSubmit()
    if (validationError) {
      alert(validationError)
      return
    }

    await projectsStore.create({
      name:             formData.value.name,
      startDate:        formData.value.startDate,
      endDate:          formData.value.endDate,
      priority:         formData.value.priority,
      customerCompany:  formData.value.customerCompany,
      executorCompany:  formData.value.executorCompany,
      projectManagerId: formData.value.projectManagerId,
    })

    // Add employees to created project / Добавляем сотрудников в созданный проект
    const created = projectsStore.projects[0]
    if (created) {
      for (const empId of formData.value.employeeIds) {
        await projectsStore.addEmployee(created.id, empId)
      }
    }

    router.push('/projects')
  } catch (e: any) {
    alert(e.response?.data?.message ?? 'Failed to create project')
  }
}
</script>

<template>
  <div class="max-w-2xl mx-auto">
    <h1 class="text-2xl font-bold mb-6">Create New Project</h1>

    <!-- Step indicator / Индикатор шагов -->
    <div class="flex items-center mb-8 gap-2">
      <div
        v-for="step in totalSteps"
        :key="step"
        class="flex-1 h-2 rounded-full transition-colors"
        :class="step <= currentStep ? 'bg-blue-700' : 'bg-gray-200'"
      />
    </div>

    <!-- Steps / Шаги -->
    <WizardStep1 v-if="currentStep === 1" v-model="formData" @next="nextStep" />
    <WizardStep2 v-if="currentStep === 2" v-model="formData" @next="nextStep" @prev="prevStep" />
    <WizardStep3 v-if="currentStep === 3" v-model="formData" @next="nextStep" @prev="prevStep" />
    <WizardStep4 v-if="currentStep === 4" v-model="formData" @next="nextStep" @prev="prevStep" />
    <WizardStep5 v-if="currentStep === 5" v-model="formData" @prev="prevStep" @submit="submitWizard" />
  </div>
</template>