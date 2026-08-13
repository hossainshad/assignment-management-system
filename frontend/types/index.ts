export interface User {
  id: string;
  fullName: string;
  email: string;
  role: "Admin" | "Teacher" | "Student";
  isActive: boolean;
  createdAt: string;
}

export interface LoginResponse {
  token: string;
  fullName: string;
  email: string;
  role: string;
}

export interface Assignment {
  id: string;
  title: string;
  description: string;
  deadline: string;
  maxMarks: number;
  status: "Draft" | "Published";
  subject: string;
  class: string;
  teacher: string;
  submissionCount?: number;
  mySubmission?: Submission | null;
  createdAt: string;
}

export interface Submission {
  id: string;
  answer: string;
  status: "Submitted" | "Late" | "Reviewed";
  marks: number | null;
  feedback: string | null;
  submittedAt: string;
}