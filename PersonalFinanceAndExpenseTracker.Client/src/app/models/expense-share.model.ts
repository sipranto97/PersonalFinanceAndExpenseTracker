export interface CreateExpenseShareLinkPayload {
  startDate: string;
  endDate: string;
  expiresAt: string | null;
}

export interface SharedExpenseItem {
  category: string;
  amount: number;
  trxDate: Date;
}

export interface SharedExpenseHistory {
  ownerName: string;
  currency: string;
  startDate: Date;
  endDate: Date;
  expiresAt: Date | null;
  expenses: SharedExpenseItem[];
}
