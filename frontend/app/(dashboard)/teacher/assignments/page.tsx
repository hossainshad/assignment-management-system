"use client";

import { useEffect, useState } from "react";
import DashboardLayout from "@/components/DashboardLayout";
import api from "@/lib/api";

interface Assignment {
  id: string;
  title: string;
  deadline: string;
  maxMarks: number;
  status: string;
  subject: string;
  class: string;
  submissionCount: number;
  createdAt: string;
}

interface Subject {
  id: string;
  name: string;
  code: string;
}

export default function TeacherAssignments() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    title: "",
    description: "",
    deadline: "",
    maxMarks: 100,
    publish: false,
    subjectId: "",
  });

  const fetchAssignments = () => {
    api.get("/api/assignment/my")
      .then((res) => setAssignments(res.data))
      .catch((err) => console.error(err.response?.data))
      .finally(() => setLoading(false));
  };

  const fetchSubjects = () => {
    api.get("/api/assignment/teacher/subjects")
      .then((res) => setSubjects(res.data))
      .catch((err) => console.error(err.response?.data));
  };

  useEffect(() => {
    fetchAssignments();
    fetchSubjects();
  }, []);

  const resetForm = () => {
    setForm({ title: "", description: "", deadline: "", maxMarks: 100, publish: false, subjectId: "" });
    setEditingId(null);
    setShowForm(false);
    setError("");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.title || !form.description || !form.deadline || !form.subjectId) {
      setError("All fields are required");
      return;
    }
    setSaving(true);
    setError("");
    try {
      if (editingId) {
        await api.put(`/api/assignment/${editingId}`, form);
      } else {
        await api.post("/api/assignment", form);
      }
      resetForm();
      fetchAssignments();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg || "Failed to save assignment");
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = (a: Assignment) => {
    setEditingId(a.id);
    setForm({
      title: a.title,
      description: "",
      deadline: new Date(a.deadline).toISOString().slice(0, 16),
      maxMarks: a.maxMarks,
      publish: a.status === "Published",
      subjectId: "",
    });
    setShowForm(true);
    setError("");
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Delete this assignment?")) return;
    await api.delete(`/api/assignment/${id}`);
    fetchAssignments();
  };

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-gray-800">My Assignments</h1>
          <button
            onClick={() => { setShowForm(!showForm); setEditingId(null); setError(""); }}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 transition"
          >
            {showForm ? "Cancel" : "+ New Assignment"}
          </button>
        </div>

        {showForm && (
          <form onSubmit={handleSubmit} className="bg-white border border-gray-200 rounded-xl p-5 mb-6 space-y-3">
            <h2 className="font-semibold text-gray-700">
              {editingId ? "Edit Assignment" : "Create Assignment"}
            </h2>

            {error && <p className="text-red-500 text-sm">{error}</p>}

            <input
              placeholder="Title"
              value={form.title}
              onChange={(e) => setForm({ ...form, title: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <textarea
              placeholder="Description"
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              rows={4}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
            <div className="flex gap-3">
              <input
                type="datetime-local"
                value={form.deadline}
                onChange={(e) => setForm({ ...form, deadline: e.target.value })}
                className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <input
                type="number"
                placeholder="Max Marks"
                value={form.maxMarks}
                onChange={(e) => setForm({ ...form, maxMarks: parseInt(e.target.value) })}
                className="w-32 border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <select
              value={form.subjectId}
              onChange={(e) => setForm({ ...form, subjectId: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select Subject</option>
              {subjects.map((s) => (
                <option key={s.id} value={s.id}>{s.name} ({s.code})</option>
              ))}
            </select>
            <label className="flex items-center gap-2 text-sm text-gray-700 cursor-pointer">
              <input
                type="checkbox"
                checked={form.publish}
                onChange={(e) => setForm({ ...form, publish: e.target.checked })}
                className="rounded"
              />
              Publish immediately
            </label>
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition"
            >
              {saving ? "Saving..." : editingId ? "Update" : "Create"}
            </button>
          </form>
        )}

        {loading && <p className="text-gray-400">Loading...</p>}
        {!loading && assignments.length === 0 && (
          <p className="text-gray-400">No assignments yet. Create one!</p>
        )}

        <div className="space-y-4">
          {assignments.map((a) => (
            <div key={a.id} className="bg-white border border-gray-200 rounded-xl p-5">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <h2 className="font-semibold text-gray-800">{a.title}</h2>
                  <p className="text-sm text-gray-500 mt-1">{a.subject} — {a.class}</p>
                </div>
                <span className={`text-xs px-2 py-1 rounded-full font-medium shrink-0 ${
                  a.status === "Published"
                    ? "bg-green-100 text-green-700"
                    : "bg-yellow-100 text-yellow-700"
                }`}>
                  {a.status}
                </span>
              </div>

              <div className="mt-3 flex items-center justify-between">
                <div className="flex gap-4 text-xs text-gray-400">
                  <span>Due: {new Date(a.deadline).toLocaleDateString("en-GB", {
                    day: "numeric", month: "short", year: "numeric"
                  })}</span>
                  <span>Max: {a.maxMarks}</span>
                  <span>{a.submissionCount} submissions</span>
                </div>

                <div className="flex gap-3">
                <a
                    href={`/teacher/assignments/${a.id}/submissions`}
                    className="text-xs text-blue-600 hover:underline"
                  >
                    View submissions
                  </a>
                  <button
                    onClick={() => handleEdit(a)}
                    className="text-xs text-gray-600 hover:underline"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(a.id)}
                    className="text-xs text-red-500 hover:underline"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}