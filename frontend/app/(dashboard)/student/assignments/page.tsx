"use client";

import { useEffect, useState } from "react";
import DashboardLayout from "@/components/DashboardLayout";
import api from "@/lib/api";
import { Assignment } from "@/types";

export default function StudentAssignments() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get("/api/assignment/class")
      .then((res) => setAssignments(res.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const isOverdue = (deadline: string) => new Date(deadline) < new Date();

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">My Assignments</h1>

        {loading && <p className="text-gray-400">Loading...</p>}

        {!loading && assignments.length === 0 && (
          <p className="text-gray-400">No assignments yet.</p>
        )}

        <div className="space-y-4">
          {assignments.map((a) => (
            <div key={a.id} className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <h2 className="font-semibold text-gray-800">{a.title}</h2>
                  <p className="text-sm text-gray-500 mt-1">{a.subject} — {a.class}</p>
                  <p className="text-sm text-gray-500">Teacher: {a.teacher}</p>
                </div>

                {/* Submission status badge */}
                {a.mySubmission ? (
                  <span className={`text-xs px-2 py-1 rounded-full font-medium shrink-0 ${
                    a.mySubmission.status === "Reviewed"
                      ? "bg-green-100 text-green-700"
                      : a.mySubmission.status === "Late"
                      ? "bg-red-100 text-red-700"
                      : "bg-blue-100 text-blue-700"
                  }`}>
                    {a.mySubmission.status}
                  </span>
                ) : (
                  <span className="text-xs px-2 py-1 rounded-full font-medium shrink-0 bg-yellow-100 text-yellow-700">
                    Not submitted
                  </span>
                )}
              </div>

              <div className="mt-3 flex items-center justify-between">
                <p className={`text-xs ${isOverdue(a.deadline) ? "text-red-500" : "text-gray-400"}`}>
                  Due: {new Date(a.deadline).toLocaleDateString("en-GB", {
                    day: "numeric", month: "short", year: "numeric", hour: "2-digit", minute: "2-digit"
                  })}
                </p>
                <p className="text-xs text-gray-400">Max marks: {a.maxMarks}</p>
              </div>

              {/* Marks and feedback if reviewed */}
              {a.mySubmission?.status === "Reviewed" && (
                <div className="mt-3 p-3 bg-green-50 rounded-lg text-sm">
                  <p className="font-medium text-green-700">
                    Marks: {a.mySubmission.marks} / {a.maxMarks}
                  </p>
                  {a.mySubmission.feedback && (
                    <p className="text-green-600 mt-1">{a.mySubmission.feedback}</p>
                  )}
                </div>
              )}

              {/* Submit button */}
              {!a.mySubmission && !isOverdue(a.deadline) && (
                <a
                  href={`/student/assignments/${a.id}`}
                  className="mt-3 inline-block text-sm text-blue-600 hover:underline"
                >
                  Submit answer →
                </a>
              )}
            </div>
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}