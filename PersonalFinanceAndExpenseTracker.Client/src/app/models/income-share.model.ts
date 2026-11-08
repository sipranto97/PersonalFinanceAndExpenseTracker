export interface CreateShareLinkPayload {
  startDate: string;
  endDate: string;
  expiresAt: string | null;
}

export interface SharedIncomeItem {
  category: string;
  amount: number;
  trxDate: Date;
}

export interface SharedIncomeHistory {
  ownerName: string;
  currency: string;
  startDate: Date;
  endDate: Date;
  expiresAt: Date | null;
  incomes: SharedIncomeItem[];
}
