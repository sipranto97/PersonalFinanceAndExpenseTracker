import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { SavingsGoalService } from '../../services/savings-goal.service';
import { SavingsGoal } from '../../models/savings-goal.model';
import { AddSavingsGoalModalComponent } from '../add-savings-goal-modal/add-savings-goal-modal.component';

@Component({
    selector: 'app-savings-goal',
    templateUrl: './savings-goal.component.html',
    styleUrl: './savings-goal.component.css',
    standalone: false
})
export class SavingsGoalComponent implements OnInit {
  goals: SavingsGoal[] = [];
  contributionAmounts: { [goalId: number]: number | null } = {};

  constructor(
    private dialog: MatDialog,
    private savingsGoalService: SavingsGoalService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getGoals();
  }

  getGoals(): void {
    this.savingsGoalService.getGoals().subscribe((goals: SavingsGoal[]) => {
      this.goals = goals;
    });
  }

  openAddGoalModal(): void {

    const dialogRef = this.dialog.open(
      AddSavingsGoalModalComponent,
      {
        width: '480px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.savingsGoalService.createGoal(result).subscribe(() => {
        this.toastr.success('Savings goal created successfully.');
        this.getGoals();
      });

    });
  }

  contribute(goal: SavingsGoal): void {
    const amount = this.contributionAmounts[goal.id];

    if (!amount || amount < 1) {
      this.toastr.error('Please enter a valid contribution amount.');
      return;
    }

    this.savingsGoalService.addContribution(goal.id, { amount: Number(amount) }).subscribe({
      next: (res) => {
        this.toastr.success('Contribution added successfully.');

        if (res.isJustCompleted) {
          this.toastr.success(`"${goal.name}" reached its target!`);
        }

        this.contributionAmounts[goal.id] = null;
        this.getGoals();
      },
      error: (err) => {
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to add this contribution.');
      }
    });
  }

  settle(goal: SavingsGoal, action: 'AddToBalance' | 'Expense'): void {
    this.savingsGoalService.settleGoal(goal.id, { action }).subscribe(() => {
      this.toastr.success('Goal settled successfully.');
      this.getGoals();
    });
  }

  deleteGoal(id: number): void {
    this.savingsGoalService.deleteGoal(id).subscribe(() => {
      this.toastr.success('Goal deleted successfully.');
      this.getGoals();
    });
  }
}
