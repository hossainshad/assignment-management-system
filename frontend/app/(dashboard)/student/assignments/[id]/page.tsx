"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import DashboardLayout from "@/components/DashboardLayout";
import api from "@/lib/api";

interface AssignmentDetail {
  id: string;
  title: string;
  description: string;
  deadline: string;
  maxMarks: number;
  subject: { name: string };
  class: { name: string; section: string };
  teacher: string;
}

export default function SubmitAssignment() {
  const { id } = useParams();
  const router = useRouter();
  const [assignment, setAssignment] = useState<AssignmentDetail | null>(null);
  const [answer, setAnswer] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    api.get(`/api/assignment/${id}`)
      .then((res) => setAssignment(res.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!answer.trim()) {
      setError("Answer cannot be empty");
      return;
    }
    setSubmitting(true);
    setError("");
    try {
      await api.post("/api/submission", {
        assignmentId: id,
        answer: answer.trim(),
      });
      router.push("/student/assignments");
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message;
      setError(msg || "Failed to submit");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <DashboardLayout><p className="text-gray-400">Loading...</p></DashboardLayout>;
  if (!assignment) return <DashboardLayout><p className="text-gray-400">Assignment not found.</p></DashboardLayout>;

  const isOverdue = new Date(assignment.deadline) < new Date();

  return (
    <DashboardLayout>
      <div className="max-w-2xl mx-auto">
        <button
          onClick={() => router.back()}
          className="text-sm text-gray-400 hover:text-gray-600 mb-4 inline-block"
        >
          ← Back
        </button>

        <div className="bg-white rounded-xl border border-gray-200 p-6 mb-4">
          <h1 className="text-xl font-bold text-gray-800">{assignment.title}</h1>
          <p className="text-sm text-gray-500 mt-1">
            {assignment.subject.name} — {assignment.class.name} {assignment.class.section}
          </p>
          <p className="text-sm text-gray-500">Teacher: {assignment.teacher}</p>

          <div className="mt-4 p-4 bg-gray-50 rounded-lg">
            <p className="text-sm text-gray-700">{assignment.description}</p>
          </div>

          <div className="mt-3 flex gap-4 text-sm">
            <p className={isOverdue ? "text-red-500" : "text-gray-500"}>
              Due: {new Date(assignment.deadline).toLocaleDateString("en-GB", {
                day: "numeric", month: "short", year: "numeric",
                hour: "2-digit", minute: "2-digit"
              })}
            </p>
            <p className="text-gray-500">Max marks: {assignment.maxMarks}</p>
          </div>
        </div>

        {isOverdue ? (
          <div className="bg-red-50 text-red-600 px-4 py-3 rounded-lg text-sm">
            The deadline has passed. You can no longer submit.
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="bg-white rounded-xl border border-gray-200 p-6">
            <h2 className="font-semibold text-gray-800 mb-3">Your Answer</h2>

            {error && (
              <div className="bg-red-50 text-red-600 px-4 py-3 rounded-lg mb-4 text-sm">
                {error}
              </div>
            )}

            <textarea
              value={answer}
              onChange={(e) => setAnswer(e.target.value)}
              rows={8}
              placeholder="Write your answer here..."
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />

            <button
              type="submit"
              disabled={submitting}
              className="mt-4 bg-blue-600 text-white px-6 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition"
            >
              {submitting ? "Submitting..." : "Submit Assignment"}
            </button>
          </form>
        )}
      </div>
    </DashboardLayout>
  );
}