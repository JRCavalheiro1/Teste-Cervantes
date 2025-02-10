SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;


CREATE TABLE public.cadastro (
    id_cadastro integer NOT NULL,
    texto character varying(255) NOT NULL,
    numero integer NOT NULL,
    CONSTRAINT cadastro_numero_check CHECK ((numero > 0))
);


ALTER TABLE public.cadastro OWNER TO postgres;


CREATE SEQUENCE public.cadastro_id_cadastro_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cadastro_id_cadastro_seq OWNER TO postgres;


ALTER SEQUENCE public.cadastro_id_cadastro_seq OWNED BY public.cadastro.id_cadastro;


CREATE TABLE public.log_operacoes (
    id integer NOT NULL,
    data_hora timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    operacao character varying(10) NOT NULL,
    numero integer NOT NULL
);


ALTER TABLE public.log_operacoes OWNER TO postgres;

CREATE SEQUENCE public.log_operacoes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.log_operacoes_id_seq OWNER TO postgres;


ALTER SEQUENCE public.log_operacoes_id_seq OWNED BY public.log_operacoes.id;


ALTER TABLE ONLY public.cadastro ALTER COLUMN id_cadastro SET DEFAULT nextval('public.cadastro_id_cadastro_seq'::regclass);


ALTER TABLE ONLY public.log_operacoes ALTER COLUMN id SET DEFAULT nextval('public.log_operacoes_id_seq'::regclass);


ALTER TABLE ONLY public.cadastro
    ADD CONSTRAINT cadastro_numero_key UNIQUE (numero);


ALTER TABLE ONLY public.cadastro
    ADD CONSTRAINT cadastro_pkey PRIMARY KEY (id_cadastro);


ALTER TABLE ONLY public.log_operacoes
    ADD CONSTRAINT log_operacoes_pkey PRIMARY KEY (id);

