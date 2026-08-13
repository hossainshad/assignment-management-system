--
-- PostgreSQL database dump
--

\restrict rU8Gcbr6cc1G8l6qhdeRdiNeqeL4RulphPE9IufRKYupXIDhx1ZY7NJAxLSmpPy

-- Dumped from database version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Assignments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Assignments" (
    "Id" uuid NOT NULL,
    "Title" text NOT NULL,
    "Description" text NOT NULL,
    "Deadline" timestamp with time zone NOT NULL,
    "MaxMarks" integer NOT NULL,
    "Status" text NOT NULL,
    "SubjectId" uuid NOT NULL,
    "TeacherId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Assignments" OWNER TO postgres;

--
-- Name: ClassEnrollments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ClassEnrollments" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "ClassId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."ClassEnrollments" OWNER TO postgres;

--
-- Name: Classes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Classes" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Section" text NOT NULL,
    "Description" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Classes" OWNER TO postgres;

--
-- Name: Subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Subjects" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Code" text NOT NULL,
    "ClassId" uuid NOT NULL,
    "TeacherId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Subjects" OWNER TO postgres;

--
-- Name: Submissions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Submissions" (
    "Id" uuid NOT NULL,
    "Answer" text NOT NULL,
    "Status" text NOT NULL,
    "Marks" integer,
    "Feedback" text,
    "SubmittedAt" timestamp with time zone NOT NULL,
    "AssignmentId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Submissions" OWNER TO postgres;

--
-- Name: Users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Users" (
    "Id" uuid NOT NULL,
    "FullName" text NOT NULL,
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" text NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Users" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: Assignments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Assignments" ("Id", "Title", "Description", "Deadline", "MaxMarks", "Status", "SubjectId", "TeacherId", "CreatedAt", "UpdatedAt") FROM stdin;
44eb0a2f-f063-42ff-b783-527f8318fc48	Algebra Basics	Solve algebra problems	2026-08-21 05:59:00+06	100	Published	cc061abb-d71d-41da-8812-2b8b4339beb2	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 20:36:24.513998+06	2026-08-08 20:36:24.513998+06
1a0b0c09-86a1-4c71-bc6c-7bba4989dafe	Algebra Basics	Solve algebra problems	2026-08-21 05:59:00+06	100	Published	cc061abb-d71d-41da-8812-2b8b4339beb2	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 20:37:32.790333+06	2026-08-08 20:37:32.790333+06
b02f31fb-9b4f-4385-b218-0ed18cab901b	Algebra Basics	Solve algebra problems	2026-08-21 05:59:00+06	100	Published	cc061abb-d71d-41da-8812-2b8b4339beb2	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 20:38:56.368904+06	2026-08-08 20:38:56.368904+06
4901c0ac-b5ce-4457-a411-92750880cdb5	Algebra Basics	Solve algebra problems	2026-08-21 05:59:00+06	100	Published	cc061abb-d71d-41da-8812-2b8b4339beb2	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 20:40:47.070519+06	2026-08-08 20:40:47.070519+06
\.


--
-- Data for Name: ClassEnrollments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."ClassEnrollments" ("Id", "StudentId", "ClassId", "CreatedAt", "UpdatedAt") FROM stdin;
ae1ad7ec-fdfd-4b13-9d71-80104f4a248d	47d67050-f939-4b47-97e4-ef5b77bf7693	8bf457a2-3ef4-48a0-bbd4-b61dcd74e424	2026-08-08 20:13:38.251454+06	2026-08-08 20:13:38.251454+06
\.


--
-- Data for Name: Classes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Classes" ("Id", "Name", "Section", "Description", "CreatedAt", "UpdatedAt") FROM stdin;
8bf457a2-3ef4-48a0-bbd4-b61dcd74e424	Class 10	A	Demo class for testing	2026-08-08 20:13:38.081405+06	2026-08-08 20:13:38.081405+06
b2507af3-1998-4408-be5d-d6c5a619e0be	Class 9	A		2026-08-08 21:32:18.761591+06	2026-08-08 21:32:18.761591+06
\.


--
-- Data for Name: Subjects; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Subjects" ("Id", "Name", "Code", "ClassId", "TeacherId", "CreatedAt", "UpdatedAt") FROM stdin;
cc061abb-d71d-41da-8812-2b8b4339beb2	Mathematics	MATH101	8bf457a2-3ef4-48a0-bbd4-b61dcd74e424	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 20:13:38.203629+06	2026-08-08 20:13:38.203629+06
61a87253-13b9-4fac-9c66-d3af7d38fb0e	Science	SC101	b2507af3-1998-4408-be5d-d6c5a619e0be	b838a50d-76ae-4d92-a33c-4205483e81f3	2026-08-08 21:32:39.480709+06	2026-08-08 21:32:39.480709+06
\.


--
-- Data for Name: Submissions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Submissions" ("Id", "Answer", "Status", "Marks", "Feedback", "SubmittedAt", "AssignmentId", "StudentId", "CreatedAt", "UpdatedAt") FROM stdin;
295aae28-a51f-4488-9125-4e324d928e85	yess	Submitted	\N	\N	2026-08-08 21:27:53.467415+06	4901c0ac-b5ce-4457-a411-92750880cdb5	47d67050-f939-4b47-97e4-ef5b77bf7693	2026-08-08 21:27:53.467258+06	2026-08-08 21:27:53.467258+06
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Users" ("Id", "FullName", "Email", "PasswordHash", "Role", "IsActive", "CreatedAt", "UpdatedAt") FROM stdin;
30aeda63-6f89-438f-a049-fdc7355349a0	System Admin	admin@school.com	$2a$11$xj/0BsXyD5ZC4ji2kD6hve5ZzKCQ1IzgppIdgi53R9kPfTBI9ar0i	Admin	t	2026-08-08 20:13:37.48351+06	2026-08-08 20:13:37.48351+06
47d67050-f939-4b47-97e4-ef5b77bf7693	Jane Student	student@school.com	$2a$11$U40KHdeJmItKQzs.s4vbDOI2muMG4PpZLAh/eWVZPeHLLcOhenkXi	Student	t	2026-08-08 20:13:37.862206+06	2026-08-08 20:13:37.862206+06
b838a50d-76ae-4d92-a33c-4205483e81f3	John Teacher	teacher@school.com	$2a$11$VS2bTMr865/5tmBlp7PPP.XA84jFeJtpmmn7f0AINBNWHiVIs5bGq	Teacher	t	2026-08-08 20:13:37.733237+06	2026-08-08 20:13:37.733237+06
74d23dfe-def9-4b46-b632-3181e6e932d0	Sazzad Hossain	shad@gmail.com	$2a$11$t6UglnrL0rRvGRU9e0Q6EOW5C3routaSbAGwHWoMA0piBQqIoahqi	Student	t	2026-08-08 21:31:12.068966+06	2026-08-08 21:31:12.068966+06
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260808141012_InitialCreate	10.0.10
\.


--
-- Name: Assignments PK_Assignments; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assignments"
    ADD CONSTRAINT "PK_Assignments" PRIMARY KEY ("Id");


--
-- Name: ClassEnrollments PK_ClassEnrollments; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClassEnrollments"
    ADD CONSTRAINT "PK_ClassEnrollments" PRIMARY KEY ("Id");


--
-- Name: Classes PK_Classes; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Classes"
    ADD CONSTRAINT "PK_Classes" PRIMARY KEY ("Id");


--
-- Name: Subjects PK_Subjects; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subjects"
    ADD CONSTRAINT "PK_Subjects" PRIMARY KEY ("Id");


--
-- Name: Submissions PK_Submissions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Submissions"
    ADD CONSTRAINT "PK_Submissions" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Assignments_SubjectId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Assignments_SubjectId" ON public."Assignments" USING btree ("SubjectId");


--
-- Name: IX_Assignments_TeacherId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Assignments_TeacherId" ON public."Assignments" USING btree ("TeacherId");


--
-- Name: IX_ClassEnrollments_ClassId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ClassEnrollments_ClassId" ON public."ClassEnrollments" USING btree ("ClassId");


--
-- Name: IX_ClassEnrollments_StudentId_ClassId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_ClassEnrollments_StudentId_ClassId" ON public."ClassEnrollments" USING btree ("StudentId", "ClassId");


--
-- Name: IX_Subjects_ClassId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Subjects_ClassId" ON public."Subjects" USING btree ("ClassId");


--
-- Name: IX_Subjects_TeacherId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Subjects_TeacherId" ON public."Subjects" USING btree ("TeacherId");


--
-- Name: IX_Submissions_AssignmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Submissions_AssignmentId" ON public."Submissions" USING btree ("AssignmentId");


--
-- Name: IX_Submissions_StudentId_AssignmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Submissions_StudentId_AssignmentId" ON public."Submissions" USING btree ("StudentId", "AssignmentId");


--
-- Name: IX_Users_Email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Users_Email" ON public."Users" USING btree ("Email");


--
-- Name: Assignments FK_Assignments_Subjects_SubjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assignments"
    ADD CONSTRAINT "FK_Assignments_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES public."Subjects"("Id") ON DELETE CASCADE;


--
-- Name: Assignments FK_Assignments_Users_TeacherId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assignments"
    ADD CONSTRAINT "FK_Assignments_Users_TeacherId" FOREIGN KEY ("TeacherId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: ClassEnrollments FK_ClassEnrollments_Classes_ClassId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClassEnrollments"
    ADD CONSTRAINT "FK_ClassEnrollments_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES public."Classes"("Id") ON DELETE CASCADE;


--
-- Name: ClassEnrollments FK_ClassEnrollments_Users_StudentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClassEnrollments"
    ADD CONSTRAINT "FK_ClassEnrollments_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Subjects FK_Subjects_Classes_ClassId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subjects"
    ADD CONSTRAINT "FK_Subjects_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES public."Classes"("Id") ON DELETE CASCADE;


--
-- Name: Subjects FK_Subjects_Users_TeacherId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subjects"
    ADD CONSTRAINT "FK_Subjects_Users_TeacherId" FOREIGN KEY ("TeacherId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: Submissions FK_Submissions_Assignments_AssignmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Submissions"
    ADD CONSTRAINT "FK_Submissions_Assignments_AssignmentId" FOREIGN KEY ("AssignmentId") REFERENCES public."Assignments"("Id") ON DELETE CASCADE;


--
-- Name: Submissions FK_Submissions_Users_StudentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Submissions"
    ADD CONSTRAINT "FK_Submissions_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict rU8Gcbr6cc1G8l6qhdeRdiNeqeL4RulphPE9IufRKYupXIDhx1ZY7NJAxLSmpPy

