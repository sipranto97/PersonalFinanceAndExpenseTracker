export interface Category {
  id: number;
  name: string;
}

export interface Income {
  id: number;
  categoryId: number;
  category: string;
  amount: number;
  trxDate: Date;
}