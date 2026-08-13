"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import DashboardLayout from "@/components/DashboardLayout";
import api from "@/lib/api";

interface Submission {
  id: string;
  student: { fullName: string; email: string };
  answer: string;
  status: string;
  marks: number | null;
  feedback: string | null;
  submittedAt: string;
}

export default function TeacherSubmissions() {
  const { id } = useParams();
  const router = useRouter();
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [loading, setLoading] = useState(true);
  const [gradingId, setGradingId] = useState<string | null>(null);
  const [gradeForm, setGradeForm] = useState({ marks: 0, feedback: "" });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const fetchSubmissions = () => {
    api.get(`/api/submission/assignment/${id}`)
      .then((res) => setSubmissions(res.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => { fetchSubmissions(); }, [id]);

  const handleGrade = async (submissionId: string) => {
    setSaving(true);
    setError("");
    try {
      await api.patch(`/api/submission/${submissionId}/grade`, gradeForm);
      setGradingId(null);
      setGradeForm({ marks: 0, feedback: "" });
      fetchSubmissions();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg || "Failed to grade");
    } finally {
      setSaving(false);
    }
  };

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        <button
          onClick={() => router.back()}
          className="text-sm text-gray-400 hover:text-gray-600 mb-4 inline-block"
        >
          ← Back
        </button>

        <h1 className="text-2xl font-bold text-gray-800 mb-6">Submissions</h1>

        {loading && <p className="text-gray-400">Loading...</p>}
        {!loading && submissions.length === 0 && (
          <p className="text-gray-400">No submissions yet.</p>
        )}

        <div className="space-y-4">
          {submissions.map((s) => (
            <div key={s.id} className="bg-white border border-gray-200 rounded-xl p-5">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="font-semibold text-gray-800">{s.student.fullName}</p>
                  <p className="text-xs text-gray-400">{s.student.email}</p>
                  <p className="text-xs text-gray-400 mt-1">
                    Submitted: {new Date(s.submittedAt).toLocaleDateString("en-GB", {
                      day: "numeric", month: "short", year: "numeric",
                      hour: "2-digit", minute: "2-digit"
                    })}
                  </p>
                </div>
                <span className={`text-xs px-2 py-1 rounded-full font-medium shrink-0 ${
                  s.status === "Reviewed" ? "bg-green-100 text-green-700" :
                  s.status === "Late" ? "bg-red-100 text-red-700" :
                  "bg-blue-100 text-blue-700"
                }`}>
                  {s.status}
                </span>
              </div>

              {/* Answer */}
              <div className="mt-3 p-3 bg-gray-50 rounded-lg">
                <p className="text-xs text-gray-400 mb-1">Answer:</p>
                <p className="text-sm text-gray-700 whitespace-pre-wrap">{s.answer}</p>
              </div>

              {/* Already graded */}
              {s.status === "Reviewed" && (
                <div className="mt-3 p-3 bg-green-50 rounded-lg">
                  <p className="text-sm font-medium text-green-700">Marks: {s.marks}</p>
                  {s.feedback && <p className="text-sm text-green-600 mt-1">{s.feedback}</p>}
                </div>
              )}

              {/* Grade form */}
              {gradingId === s.id ? (
                <div className="mt-3 space-y-2">
                  {error && <p className="text-red-500 text-xs">{error}</p>}
                  <input
                    type="number"
                    placeholder="Marks"
                    value={gradeForm.marks}
                    onChange={(e) => setGradeForm({ ...gradeForm, marks: parseInt(e.target.value) })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                  <textarea
                    placeholder="Feedback (optional)"
                    value={gradeForm.feedback}
                    onChange={(e) => setGradeForm({ ...gradeForm, feedback: e.target.value })}
                    rows={3}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                  />
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleGrade(s.id)}
                      disabled={saving}
                      className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition"
                    >
                      {saving ? "Saving..." : "Save Grade"}
                    </button>
                    <button
                      onClick={() => setGradingId(null)}
                      className="text-sm text-gray-400 hover:text-gray-600 px-3"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              ) : (
                <button
                  onClick={() => {
                    setGradingId(s.id);
                    setGradeForm({ marks: s.marks ?? 0, feedback: s.feedback ?? "" });
                  }}
                  className="mt-3 text-sm text-blue-600 hover:underline"
                >
                  {s.status === "Reviewed" ? "Update grade" : "Grade this submission"}
                </button>
              )}
            </div>
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}