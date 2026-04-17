import type { Employee } from '@/types'

/** Sort by display name for stable “first 5” preview. */
export function sortEmployeesByName(employees: Employee[]): Employee[] {
  return [...employees].sort((a, b) =>
    a.fullName.localeCompare(b.fullName, undefined, { sensitivity: 'base' }),
  )
}

/**
 * Picker list: empty query → first `previewCount` employees (after sort, minus excluded).
 * Non-empty query → employees whose first name, last name, full name, or email starts with the prefix (case-insensitive).
 */
export function employeesForPicker(
  all: Employee[],
  query: string,
  excludeIds: Set<string>,
  previewCount = 5,
): Employee[] {
  const sorted = sortEmployeesByName(all).filter((e) => !excludeIds.has(e.id))
  const q = query.trim().toLowerCase()
  if (!q) return sorted.slice(0, previewCount)

  return sorted.filter((e) => {
    const fn = e.firstName.toLowerCase()
    const ln = e.lastName.toLowerCase()
    const full = e.fullName.toLowerCase()
    const em = e.email.toLowerCase()
    return fn.startsWith(q) || ln.startsWith(q) || full.startsWith(q) || em.startsWith(q)
  })
}
