export interface RecurringExpense {
  id: number;
  categoryId: number;
  category: string;
  amount: number;
  dayOfMonth: number;
  isActive: boolean;
  lastGeneratedOn: Date | null;
}
