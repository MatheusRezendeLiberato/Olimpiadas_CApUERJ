# 🎯 Projeto Olimpíadas CAp-UERJ - Visão Geral Completa

Olá, equipe do Frontend! Este documento explica tudo que estamos construindo juntos.

---

## 📚 Contexto do Projeto

Estamos criando um **sistema de ranking automático para as Olimpíadas da CAp-UERJ**, onde as equipes (representadas por bandeiras/cores) competem em diferentes modalidades e acumulam pontos e medalhas.

---

## 🎯 Objetivo Geral

**Criar uma aplicação que:**

1. **Registra resultados** de competições (qual bandeira ficou em qual posição em qual modalidade)
2. **Calcula pontos automaticamente** baseado em tabelas de pontuação específicas por modalidade
3. **Conta medalhas** (Ouro, Prata, Bronze) por bandeira
4. **Gera um ranking dinâmico** ordenado por pontos totais
5. **Atualiza a tabela em tempo real** conforme novos resultados são inseridos

---

## 🏗️ Arquitetura do Projeto

```
┌─────────────────────────────────────────────────────────┐
│                    APLICAÇÃO REACT                       │
│              (Frontend - localhost:3000)                 │
├─────────────────────────────────────────────────────────┤
│  • Exibe tabela de ranking                              │
│  • Formulário para adicionar resultados                 │
│  • Botões para limpar/gerenciar dados                   │
└────────────────┬────────────────────────────────────────┘
                 │
                 │ HTTP Requests
                 │ (fetch/axios)
                 │
┌────────────────▼────────────────────────────────────────┐
│               ASP.NET Core 10 API                        │
│         (Backend - localhost:5006)                      │
├─────────────────────────────────────────────────────────┤
│  📊 Calculadora_Pontos.cs                               │
│     - Calcula pontos por modalidade                     │
│     - Conta medalhas                                     │
│     - Gera ranking                                       │
│                                                          │
│  🔌 REST Endpoints:                                      │
│     POST   /resultados   → Adiciona resultado           │
│     GET    /ranking      → Retorna ranking com medalhas │
│     GET    /resultados   → Lista todos os resultados    │
│     DELETE /resultados   → Limpa tudo                   │
│     GET    /quadro       → Dados estáticos (Quadro.json)│
└──────────────────────────────────────────────────────────┘
```

---

## 📊 O Que Estamos Fazendo

### **Backend (C# - Já Pronto ✅)**

Implementamos:

1. **Classe `Calculadora_Pontos`** que:
   - Define as 5 modalidades com suas tabelas de pontos
   - Calcula pontos por colocação/modalidade
   - Conta medalhas (Ouro = 1º, Prata = 2º, Bronze = 3º)
   - Agrega tudo por bandeira
   - Gera ranking ordenado

2. **Tabelas de Pontuação** (conforme regras das olimpíadas):
   ```
   Modalidades Individuais:        1º=4pts  2º=3pts  3º=2pts  4º=1pt
   Coletivas 1ª-4ª:               1º=16pts 2º=13pts 3º=10pts 4º=7pts
   Coletivas 1º-5º:               1º=12pts 2º=10pts 3º=8pts  4º=6pts
   Tênis Duplas (dobro):           1º=8pts  2º=6pts  3º=4pts  4º=2pts
   Adaptadas (coletiva):           1º=16pts 2º=13pts 3º=10pts 4º=7pts
   ```

3. **REST API** com endpoints funcionando:
   - POST /resultados → Registra novos resultados
   - GET /ranking → Retorna dados prontos para renderizar na tabela
   - GET/DELETE para gerenciar dados

### **Frontend (React - Precisa Fazer 🚀)**

Vocês precisam:

1. **Integrar o Ranking Dinâmico:**
   - Buscar dados do endpoint `/ranking` (em vez de arquivo estático)
   - Renderizar na tabela atual com os dados reais

2. **Criar Formulário de Entrada:**
   - Select para escolher a **Bandeira** (Amarela, Azul, Vermelha, Verde)
   - Select para escolher a **Modalidade** (5 opções)
   - Input numérico para **Colocação** (1-4)
   - Botão **"Adicionar Resultado"**

3. **Automatizar o Fluxo:**
   - Quando clicar em "Adicionar", enviar POST para `/resultados`
   - Depois de receber resposta, fazer GET `/ranking`
   - Atualizar a tabela com novos dados (posição, medalhas, pontos)

---

## 🔄 Fluxo Completo de Uso

```
1. USUÁRIO abre a página (localhost:3000)
   ↓
2. React faz GET /ranking na API
   ↓
3. API retorna ranking atual com medalhas e pontos
   ↓
4. Tabela renderiza com dados da API
   ↓
5. Usuário preenche o formulário:
   - Seleciona "Amarela"
   - Seleciona "Individual"
   - Seleciona "1º lugar"
   - Clica em "Adicionar Resultado"
   ↓
6. React faz POST /resultados
   {
     "bandeira": "Amarela",
     "modalidade": "Individual",
     "colocacao": 1,
     "quantidade": 1
   }
   ↓
7. Backend (Calculadora_Pontos) processa:
   - Busca tabela de "Individual": [4, 3, 2, 1]
   - Encontra 1º lugar: +4 pontos para Amarela
   - Conta +1 Ouro para Amarela
   ↓
8. Backend retorna sucesso
   ↓
9. React faz GET /ranking novamente
   ↓
10. API retorna novo ranking recalculado:
    {
      "bandeira": "Amarela",
      "ouro": 1,
      "prata": 0,
      "bronze": 0,
      "pontos": 4,
      "posicao": X
    }
   ↓
11. Tabela atualiza automaticamente com novos dados
   ↓
12. Ciclo se repete para cada novo resultado
```

---

## 🎨 Como Deve Ficar a Página

```
┌─────────────────────────────────────────────────┐
│     Olimpíadas CAp-UERJ                   ⚡    │
├─────────────────────────────────────────────────┤
│ 📝 ADICIONAR RESULTADO                          │
│                                                 │
│  Bandeira: [Selecione ▼]                       │
│  Modalidade: [Selecione ▼]                     │
│  Colocação: [1 ▼]                              │
│  [Adicionar Resultado]  [Limpar Tudo]         │
│                                                 │
├─────────────────────────────────────────────────┤
│ 📊 RANKING ATUAL                                │
│                                                 │
│  Pos. │ Equipe   │ 🥇 │ 🥈 │ 🥉 │ Pontuação   │
│  ─────┼──────────┼────┼────┼────┼─────────────│
│   1   │ Amarela  │ 5  │ 3  │ 2  │ 100         │
│   2   │ Azul     │ 4  │ 4  │ 2  │ 90          │
│   3   │ Vermelha │ 3  │ 5  │ 1  │ 80          │
│   4   │ Verde    │ 2  │ 3  │ 4  │ 75          │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

## 🔗 Endpoints que Vocês Vão Usar

### **1. GET /ranking** (PRINCIPAL - USAR SEMPRE)
```
GET http://localhost:5006/ranking

Resposta:
[
  {
    "bandeira": "Amarela",
    "ouro": 5,
    "prata": 3,
    "bronze": 2,
    "pontos": 100,
    "posicao": 1
  },
  {
    "bandeira": "Azul",
    "ouro": 4,
    "prata": 4,
    "bronze": 2,
    "pontos": 90,
    "posicao": 2
  }
  // ... mais bandeiras
]
```

### **2. POST /resultados** (QUANDO ADICIONAR NOVO RESULTADO)
```
POST http://localhost:5006/resultados

Body:
[
  {
    "bandeira": "Amarela",
    "modalidade": "Individual",
    "colocacao": 1,
    "quantidade": 1
  }
]

Resposta:
{
  "mensagem": "1 resultado(s) adicionado(s)."
}
```

### **3. DELETE /resultados** (BOTÃO LIMPAR TUDO)
```
DELETE http://localhost:5006/resultados

Resposta:
{
  "mensagem": "N resultado(s) removido(s)."
}
```

### **4. GET /resultados** (OPCIONAL - VER HISTÓRICO)
```
GET http://localhost:5006/resultados

Resposta: Lista de todos os resultados registrados
```

---

## 💡 Exemplo de Código React (Pseudocódigo)

```tsx
function App() {
  const [ranking, setRanking] = useState([]);
  const [bandeira, setBandeira] = useState('');
  const [modalidade, setModalidade] = useState('');
  const [colocacao, setColocacao] = useState(1);

  // Carrega ranking ao iniciar
  useEffect(() => {
    carregarRanking();
  }, []);

  const carregarRanking = () => {
    fetch('http://localhost:5006/ranking')
      .then(res => res.json())
      .then(data => setRanking(data));
  };

  const adicionarResultado = () => {
    fetch('http://localhost:5006/resultados', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify([{
        bandeira,
        modalidade,
        colocacao,
        quantidade: 1
      }])
    })
    .then(() => {
      carregarRanking(); // Recarrega tabela
      setBandeira('');   // Limpa formulário
      setModalidade('');
      setColocacao(1);
    });
  };

  return (
    <div>
      <h1>Olimpíadas CAp-UERJ</h1>
      
      {/* Formulário */}
      <select onChange={(e) => setBandeira(e.target.value)}>
        <option>Selecione Bandeira</option>
        <option>Amarela</option>
        <option>Azul</option>
        <option>Vermelha</option>
        <option>Verde</option>
      </select>

      <select onChange={(e) => setModalidade(e.target.value)}>
        <option>Selecione Modalidade</option>
        <option value="Individual">Individual</option>
        <option value="Coletiva1a4">Coletiva 1ª-4ª</option>
        {/* ... outras modalidades */}
      </select>

      <input
        type="number"
        min="1"
        max="4"
        value={colocacao}
        onChange={(e) => setColocacao(e.target.value)}
      />

      <button onClick={adicionarResultado}>Adicionar</button>

      {/* Tabela */}
      <table>
        <thead>
          <tr>
            <th>Pos.</th>
            <th>Equipe</th>
            <th>🥇</th>
            <th>🥈</th>
            <th>🥉</th>
            <th>Pontuação</th>
          </tr>
        </thead>
        <tbody>
          {ranking.map(item => (
            <tr key={item.bandeira}>
              <td>{item.posicao}</td>
              <td>{item.bandeira}</td>
              <td>{item.ouro}</td>
              <td>{item.prata}</td>
              <td>{item.bronze}</td>
              <td>{item.pontos}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

---

## 🎯 Próximas Fases (Futuro)

**Fase 1 (AGORA):** Integrar ranking + criar formulário de entrada
**Fase 2:** Adicionar banco de dados para persistência
**Fase 3:** Adicionar autenticação/permissões
**Fase 4:** Dashboard com gráficos e estatísticas
**Fase 5:** Mobile responsivo

---

## ✅ Checklist para o Frontend

- [ ] Integrar endpoint `/ranking` para carregar tabela dinamicamente
- [ ] Criar formulário com 3 selects (Bandeira, Modalidade, Colocação)
- [ ] Implementar botão "Adicionar Resultado" com POST
- [ ] Recarregar ranking automaticamente após adicionar
- [ ] Adicionar botão "Limpar Tudo" com DELETE
- [ ] Testar fluxo completo (adicionar → ranking atualiza)
- [ ] Validar inputs do formulário
- [ ] Melhorar UX/estilos conforme necessário

---

## 🚀 Como Começar

1. **Clone/abra o projeto React**
2. **Confirme que a API está rodando:** `http://localhost:5006/ranking`
3. **Teste o endpoint no navegador** para ver se retorna dados
4. **Substitua dados estáticos** por chamadas ao `/ranking`
5. **Crie o formulário** para adicionar resultados
6. **Teste o fluxo completo**

---

## 📞 Comunicação Backend ↔ Frontend

Se houver problemas:
- ❌ Tabela não atualiza? → Verifique se `/ranking` está retornando dados
- ❌ POST falha? → Confirme URL do endpoint e estrutura do JSON
- ❌ CORS error? → Já está configurado no backend, não deve acontecer
- ❌ Dados em memória perdidos? → Normal por enquanto, será persistência futura

---

**Estamos prontos! Vocês podem começar a integração agora mesmo! 🎉**
