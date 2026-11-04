export interface SavingsGoal {
  id: number;
  name: string;
  targetAmount: number;
  targetDate: Date;
  monthlyContribution?: number | null;
  amountSaved: number;
  remainingAmount: number;
  percentageProgress: number;
  status: 'Active' | 'Completed' | 'Settled';
  settlementAction?: 'AddedToBalance' | 'Expensed' | null;
  createdAt: Date;
  paceStatus?: 'Ahead' | 'OnTrack' | 'Behind' | null;
  paceDifference?: number | null;
}

export interface ContributionResult {
  goalId: number;
  amountSaved: number;
  remainingAmount: number;
  percentageProgress: number;
  status: string;
  isJustCompleted: boolean;
}
