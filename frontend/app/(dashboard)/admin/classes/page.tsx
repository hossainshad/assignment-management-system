"use client";

import { useEffect, useState } from "react";
import DashboardLayout from "@/components/DashboardLayout";
import api from "@/lib/api";

interface Subject {
  id: string;
  name: string;
  code: string;
  teacher: string;
}

interface Class {
  id: string;
  name: string;
  section: string;
  description: string;
  studentCount: number;
  subjects: Subject[];
}

interface User {
  id: string;
  fullName: string;
  role: string;
}

export default function AdminClasses() {
  const [classes, setClasses] = useState<Class[]>([]);
  const [teachers, setTeachers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [showClassForm, setShowClassForm] = useState(false);
  const [showSubjectForm, setShowSubjectForm] = useState(false);
  const [classForm, setClassForm] = useState({ name: "", section: "", description: "" });
  const [subjectForm, setSubjectForm] = useState({ name: "", code: "", classId: "", teacherId: "" });
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  const fetchData = () => {
    Promise.all([
      api.get("/api/admin/classes"),
      api.get("/api/admin/users"),
    ]).then(([classRes, userRes]) => {
      setClasses(classRes.data);
      setTeachers(userRes.data.filter((u: User) => u.role === "Teacher"));
    }).catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => { fetchData(); }, []);

  const handleCreateClass = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!classForm.name || !classForm.section) {
      setError("Name and section are required");
      return;
    }
    setSaving(true);
    setError("");
    try {
      await api.post("/api/admin/classes", classForm);
      setClassForm({ name: "", section: "", description: "" });
      setShowClassForm(false);
      fetchData();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg || "Failed to create class");
    } finally {
      setSaving(false);
    }
  };

  const handleCreateSubject = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!subjectForm.name || !subjectForm.code || !subjectForm.classId || !subjectForm.teacherId) {
      setError("All fields are required");
      return;
    }
    setSaving(true);
    setError("");
    try {
      await api.post("/api/admin/subjects", {
        name: subjectForm.name,
        code: subjectForm.code,
        classId: subjectForm.classId,
        teacherId: subjectForm.teacherId,
      });
      setSubjectForm({ name: "", code: "", classId: "", teacherId: "" });
      setShowSubjectForm(false);
      fetchData();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg || "Failed to create subject");
    } finally {
      setSaving(false);
    }
  };

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Classes & Subjects</h1>
          <div className="flex gap-2">
            <button
              onClick={() => { setShowClassForm(!showClassForm); setShowSubjectForm(false); setError(""); }}
              className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 transition"
            >
              {showClassForm ? "Cancel" : "+ Add Class"}
            </button>
            <button
              onClick={() => { setShowSubjectForm(!showSubjectForm); setShowClassForm(false); setError(""); }}
              className="bg-green-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-green-700 transition"
            >
              {showSubjectForm ? "Cancel" : "+ Add Subject"}
            </button>
          </div>
        </div>

        {error && <p className="text-red-500 text-sm mb-4">{error}</p>}

        {/* Create Class Form */}
        {showClassForm && (
          <form onSubmit={handleCreateClass} className="bg-white border border-gray-200 rounded-xl p-5 mb-6 space-y-3">
            <h2 className="font-semibold text-gray-700">Create New Class</h2>
            <input
              placeholder="Class Name (e.g. Class 10)"
              value={classForm.name}
              onChange={(e) => setClassForm({ ...classForm, name: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
              placeholder="Section (e.g. A)"
              value={classForm.section}
              onChange={(e) => setClassForm({ ...classForm, section: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
              placeholder="Description (optional)"
              value={classForm.description}
              onChange={(e) => setClassForm({ ...classForm, description: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition"
            >
              {saving ? "Creating..." : "Create Class"}
            </button>
          </form>
        )}

        {/* Create Subject Form */}
        {showSubjectForm && (
          <form onSubmit={handleCreateSubject} className="bg-white border border-gray-200 rounded-xl p-5 mb-6 space-y-3">
            <h2 className="font-semibold text-gray-700">Create New Subject</h2>
            <input
              placeholder="Subject Name (e.g. Mathematics)"
              value={subjectForm.name}
              onChange={(e) => setSubjectForm({ ...subjectForm, name: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
              placeholder="Subject Code (e.g. MATH101)"
              value={subjectForm.code}
              onChange={(e) => setSubjectForm({ ...subjectForm, code: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <select
              value={subjectForm.classId}
              onChange={(e) => setSubjectForm({ ...subjectForm, classId: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select Class</option>
              {classes.map((c) => (
                <option key={c.id} value={c.id}>{c.name} {c.section}</option>
              ))}
            </select>
            <select
              value={subjectForm.teacherId}
              onChange={(e) => setSubjectForm({ ...subjectForm, teacherId: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select Teacher</option>
              {teachers.map((t) => (
                <option key={t.id} value={t.id}>{t.fullName}</option>
              ))}
            </select>
            <button
              type="submit"
              disabled={saving}
              className="bg-green-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-green-700 disabled:opacity-50 transition"
            >
              {saving ? "Creating..." : "Create Subject"}
            </button>
          </form>
        )}

        {loading && <p className="text-gray-400">Loading...</p>}

        <div className="space-y-4">
          {classes.map((c) => (
            <div key={c.id} className="bg-white border border-gray-200 rounded-xl p-5">
              <div className="flex items-center justify-between mb-3">
                <div>
                  <h2 className="font-semibold text-gray-800">{c.name} — Section {c.section}</h2>
                  {c.description && <p className="text-sm text-gray-500">{c.description}</p>}
                </div>
                <span className="text-xs text-gray-400">{c.studentCount} students</span>
              </div>

              {c.subjects.length === 0 ? (
                <p className="text-sm text-gray-400">No subjects yet.</p>
              ) : (
                <div className="space-y-2">
                  {c.subjects.map((s) => (
                    <div key={s.id} className="flex items-center justify-between bg-gray-50 rounded-lg px-3 py-2">
                      <div>
                        <span className="text-sm font-medium text-gray-700">{s.name}</span>
                        <span className="text-xs text-gray-400 ml-2">({s.code})</span>
                      </div>
                      <span className="text-xs text-gray-500">👤 {s.teacher}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}